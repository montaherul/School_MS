using System.Text;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Reports;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Enums;
using EmployeeEntity = SchoolManagementSystem.Models.Entities.Employee.Employee;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class AcademicReportService : IAcademicReportService
{
    private readonly IUnitOfWork _uow;
    private readonly IPdfGenerator _pdfGenerator;

    public AcademicReportService(IUnitOfWork uow, IPdfGenerator pdfGenerator)
    {
        _uow = uow;
        _pdfGenerator = pdfGenerator;
    }

    public async Task<AcademicReportViewModel> GetReportAsync(AcademicReportFilterDto filter, CancellationToken ct = default)
    {
        var model = new AcademicReportViewModel { Filter = filter };

        switch (filter.ReportType)
        {
            case "academic-year":
                model.AcademicYearReports = await GetAcademicYearReportAsync(ct);
                break;
            case "class":
                model.ClassReports = await GetClassReportAsync(ct);
                break;
            case "section":
                model.SectionReports = await GetSectionReportAsync(ct);
                break;
            case "subject":
                model.SubjectReports = await GetSubjectReportAsync(ct);
                break;
            case "teacher-load":
                model.TeacherLoadReports = await GetTeacherLoadReportAsync(ct);
                break;
            case "syllabus":
                model.SyllabusReports = await GetSyllabusReportAsync(ct);
                break;
            case "student-distribution":
                model.StudentDistribution = await GetStudentDistributionAsync(ct);
                break;
            case "capacity":
                model.CapacityReports = await GetCapacityReportAsync(ct);
                break;
        }

        return model;
    }

    private async Task<List<AcademicYearReportDto>> GetAcademicYearReportAsync(CancellationToken ct)
    {
        var repo = _uow.Repository<AcademicYear>();

        var classCounts = await _uow.Repository<SchoolClass>().Query().AsNoTracking()
            .Where(c => !c.IsDeleted)
            .GroupBy(c => true)
            .Select(g => g.Count())
            .FirstOrDefaultAsync(ct);

        var sectionCounts = await _uow.Repository<Section>().Query().AsNoTracking()
            .Where(s => !s.IsDeleted)
            .GroupBy(s => true)
            .Select(g => g.Count())
            .FirstOrDefaultAsync(ct);

        var studentCounts = await _uow.Repository<Student>().Query().AsNoTracking()
            .Where(s => !s.IsDeleted)
            .GroupBy(s => true)
            .Select(g => g.Count())
            .FirstOrDefaultAsync(ct);

        var subjectCounts = await _uow.Repository<Subject>().Query().AsNoTracking()
            .Where(s => !s.IsDeleted && s.IsActive)
            .GroupBy(s => true)
            .Select(g => g.Count())
            .FirstOrDefaultAsync(ct);

        return await repo.Query().AsNoTracking()
            .Where(y => !y.IsDeleted)
            .OrderByDescending(y => y.StartsOn)
            .Select(y => new AcademicYearReportDto
            {
                Id = y.Id,
                Name = y.Name,
                StartsOn = y.StartsOn,
                EndsOn = y.EndsOn,
                IsActive = y.IsActive,
                ClassCount = classCounts,
                SectionCount = sectionCounts,
                StudentCount = studentCounts,
                SubjectCount = subjectCounts
            })
            .ToListAsync(ct);
    }

    private async Task<List<ClassReportDto>> GetClassReportAsync(CancellationToken ct)
    {
        return await _uow.Repository<SchoolClass>()
            .ExecuteStoredProcAsync<ClassReportDto>("sp_GetAcademicClassReport");
    }

    private async Task<List<SectionReportDto>> GetSectionReportAsync(CancellationToken ct)
    {
        return await _uow.Repository<Section>()
            .ExecuteStoredProcAsync<SectionReportDto>("sp_GetAcademicSectionReport");
    }

    private async Task<List<SubjectReportDto>> GetSubjectReportAsync(CancellationToken ct)
    {
        var subjectRepo = _uow.Repository<Subject>();

        return await subjectRepo.Query().AsNoTracking()
            .Where(s => !s.IsDeleted)
            .Select(s => new SubjectReportDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                Category = s.Category,
                IsMandatory = s.IsMandatory,
                IsPractical = s.IsPractical,
                IsActive = s.IsActive,
                ClassCount = _uow.Repository<ClassSubject>().Query().AsNoTracking()
                    .Count(cs => !cs.IsDeleted && cs.SubjectId == s.Id),
                TeacherCount = _uow.Repository<ClassSubjectTeacher>().Query().AsNoTracking()
                    .Count(cst => cst.ClassSubject != null && cst.ClassSubject.SubjectId == s.Id && !cst.IsDeleted)
            })
            .OrderBy(s => s.Name)
            .ToListAsync(ct);
    }

    private async Task<List<TeacherLoadReportDto>> GetTeacherLoadReportAsync(CancellationToken ct)
    {
        var assignments = await _uow.Repository<TeacherSubjectAssignment>().Query().AsNoTracking()
            .Where(tsa => !tsa.IsDeleted && tsa.IsActive)
            .Include(tsa => tsa.Teacher)
                .ThenInclude(t => t.Employee)
            .Include(tsa => tsa.Subject)
            .Include(tsa => tsa.Class)
            .Include(tsa => tsa.Section)
            .ToListAsync(ct);

        var grouped = assignments
            .GroupBy(tsa => tsa.TeacherId)
            .Select(g =>
            {
                var first = g.First();
                return new TeacherLoadReportDto
                {
                    TeacherId = g.Key,
                    TeacherName = first.Teacher?.FullName ?? "Unknown",
                    SubjectNames = string.Join(", ", g.Select(x => x.Subject?.Name).Distinct()),
                    AssignedClasses = g.Select(x => x.ClassId).Distinct().Count(),
                    AssignedSections = g.Select(x => x.SectionId).Distinct().Count(),
                    TotalPeriodsPerWeek = g.Count()
                };
            })
            .OrderByDescending(t => t.TotalPeriodsPerWeek)
            .ToList();

        return grouped;
    }

    private async Task<List<SyllabusProgressReportDto>> GetSyllabusReportAsync(CancellationToken ct)
    {
        return await _uow.Repository<Syllabus>().Query().AsNoTracking()
            .Include(s => s.SchoolClass)
            .Include(s => s.Subject)
            .Include(s => s.AcademicYear)
            .Where(s => !s.IsDeleted)
            .OrderByDescending(s => s.UploadedAt)
            .Select(s => new SyllabusProgressReportDto
            {
                SyllabusId = s.Id,
                Title = s.Title,
                ClassName = s.SchoolClass != null ? s.SchoolClass.Name : "",
                SubjectName = s.Subject != null ? s.Subject.Name : "",
                AcademicYear = s.AcademicYear != null ? s.AcademicYear.Name : "",
                IsActive = s.IsActive,
                UploadedBy = s.UploadedBy,
                UploadedAt = s.UploadedAt
            })
            .ToListAsync(ct);
    }

    private async Task<List<StudentDistributionReportDto>> GetStudentDistributionAsync(CancellationToken ct)
    {
        return await _uow.Repository<Student>().Query().AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.Section)
            .Include(s => s.StudentGroup)
            .Where(s => !s.IsDeleted && s.Status == StudentStatus.Active)
            .GroupBy(s => new
            {
                ClassName = s.Class.Name,
                SectionName = s.Section.Name,
                GroupName = s.StudentGroup != null ? s.StudentGroup.Name : ""
            })
            .Select(g => new StudentDistributionReportDto
            {
                ClassName = g.Key.ClassName,
                SectionName = g.Key.SectionName,
                GroupName = g.Key.GroupName,
                StudentCount = g.Count()
            })
            .OrderBy(d => d.ClassName)
            .ThenBy(d => d.SectionName)
            .ToListAsync(ct);
    }

    private async Task<List<CapacityReportDto>> GetCapacityReportAsync(CancellationToken ct)
    {
        var classCapacity = await _uow.Repository<SchoolClass>().Query().AsNoTracking()
            .Where(c => !c.IsDeleted && c.IsActive)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Capacity
            })
            .ToListAsync(ct);

        var occupiedCounts = await _uow.Repository<Student>().Query().AsNoTracking()
            .Where(s => !s.IsDeleted && s.Status == StudentStatus.Active)
            .GroupBy(s => s.ClassId)
            .Select(g => new { ClassId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var occupiedMap = occupiedCounts.ToDictionary(x => x.ClassId, x => x.Count);

        var totalCapacity = classCapacity.Sum(c => c.Capacity);
        var totalOccupied = occupiedCounts.Sum(x => x.Count);

        return classCapacity.Select(c => new CapacityReportDto
        {
            ClassName = c.Name,
            TotalCapacity = c.Capacity,
            TotalOccupied = occupiedMap.GetValueOrDefault(c.Id),
            AvailableSeats = c.Capacity - occupiedMap.GetValueOrDefault(c.Id),
            OccupancyPercent = c.Capacity > 0
                ? Math.Round((double)occupiedMap.GetValueOrDefault(c.Id) / c.Capacity * 100, 1)
                : 0
        }).OrderBy(r => r.ClassName).ToList();
    }

    public async Task<byte[]> ExportPdfAsync(AcademicReportFilterDto filter, CancellationToken ct = default)
    {
        var model = await GetReportAsync(filter, ct);

        var html = new StringBuilder();
        html.Append("<html><head><meta charset='utf-8'><style>");
        html.Append("body{font-family:Arial,sans-serif;font-size:12px;margin:20px;color:#333;}");
        html.Append("h1{font-size:20px;margin-bottom:4px;}");
        html.Append("h2{font-size:16px;color:#555;margin-top:24px;}");
        html.Append("table{width:100%;border-collapse:collapse;margin-top:12px;}");
        html.Append("th,td{padding:6px 8px;border:1px solid #ddd;text-align:left;font-size:11px;}");
        html.Append("th{background:#f5f5f5;font-weight:600;}");
        html.Append("tr:nth-child(even){background:#fafafa;}");
        html.Append(".badge{display:inline-block;padding:2px 8px;border-radius:10px;font-size:10px;font-weight:600;}");
        html.Append(".badge-success{background:#e6f7e6;color:#2e7d32;}");
        html.Append(".badge-danger{background:#fde8e8;color:#c62828;}");
        html.Append("</style></head><body>");
        html.Append($"<h1>Academic Report — {filter.ReportType}</h1>");
        html.Append($"<p style='color:#888;'>Generated: {DateTime.Now:dd MMM yyyy hh:mm tt}</p>");

        AppendPdfTables(html, model, filter.ReportType);

        html.Append("</body></html>");

        return _pdfGenerator.GenerateFromHtml(html.ToString());
    }

    private static void AppendPdfTables(StringBuilder html, AcademicReportViewModel model, string reportType)
    {
        switch (reportType)
        {
            case "academic-year":
                if (model.AcademicYearReports == null || model.AcademicYearReports.Count == 0)
                    { html.Append("<p>No data found.</p>"); return; }
                html.Append("<table><thead><tr><th>Name</th><th>Starts</th><th>Ends</th><th>Classes</th><th>Sections</th><th>Students</th><th>Subjects</th><th>Status</th></tr></thead><tbody>");
                foreach (var r in model.AcademicYearReports)
                {
                    html.Append($"<tr><td>{r.Name}</td><td>{r.StartsOn:dd MMM yyyy}</td><td>{r.EndsOn:dd MMM yyyy}</td><td>{r.ClassCount}</td><td>{r.SectionCount}</td><td>{r.StudentCount}</td><td>{r.SubjectCount}</td><td>{(r.IsActive?"<span class='badge badge-success'>Active</span>":"<span class='badge badge-danger'>Inactive</span>")}</td></tr>");
                }
                html.Append("</tbody></table>");
                break;

            case "class":
                if (model.ClassReports == null || model.ClassReports.Count == 0)
                    { html.Append("<p>No data found.</p>"); return; }
                html.Append("<table><thead><tr><th>Name</th><th>Code</th><th>Sections</th><th>Students</th><th>Capacity</th><th>Occupancy</th><th>Subjects</th></tr></thead><tbody>");
                foreach (var r in model.ClassReports)
                {
                    html.Append($"<tr><td>{r.Name}</td><td>{r.Code}</td><td>{r.SectionCount}</td><td>{r.StudentCount}</td><td>{r.Capacity}</td><td>{r.OccupancyPercent}%</td><td>{r.SubjectCount}</td></tr>");
                }
                html.Append("</tbody></table>");
                break;

            case "section":
                if (model.SectionReports == null || model.SectionReports.Count == 0)
                    { html.Append("<p>No data found.</p>"); return; }
                html.Append("<table><thead><tr><th>Section</th><th>Class</th><th>Group</th><th>Capacity</th><th>Occupied</th><th>Occupancy</th></tr></thead><tbody>");
                foreach (var r in model.SectionReports)
                {
                    html.Append($"<tr><td>{r.Name}</td><td>{r.ClassName}</td><td>{r.GroupName}</td><td>{r.Capacity}</td><td>{r.Occupied}</td><td>{r.OccupancyPercent}%</td></tr>");
                }
                html.Append("</tbody></table>");
                break;

            case "subject":
                if (model.SubjectReports == null || model.SubjectReports.Count == 0)
                    { html.Append("<p>No data found.</p>"); return; }
                html.Append("<table><thead><tr><th>Code</th><th>Name</th><th>Category</th><th>Mandatory</th><th>Practical</th><th>Classes</th><th>Teachers</th></tr></thead><tbody>");
                foreach (var r in model.SubjectReports)
                {
                    html.Append($"<tr><td>{r.Code}</td><td>{r.Name}</td><td>{r.Category}</td><td>{(r.IsMandatory?"Yes":"No")}</td><td>{(r.IsPractical?"Yes":"No")}</td><td>{r.ClassCount}</td><td>{r.TeacherCount}</td></tr>");
                }
                html.Append("</tbody></table>");
                break;

            case "teacher-load":
                if (model.TeacherLoadReports == null || model.TeacherLoadReports.Count == 0)
                    { html.Append("<p>No data found.</p>"); return; }
                html.Append("<table><thead><tr><th>Teacher</th><th>Subjects</th><th>Classes</th><th>Sections</th><th>Periods/Week</th></tr></thead><tbody>");
                foreach (var r in model.TeacherLoadReports)
                {
                    html.Append($"<tr><td>{r.TeacherName}</td><td>{r.SubjectNames}</td><td>{r.AssignedClasses}</td><td>{r.AssignedSections}</td><td>{r.TotalPeriodsPerWeek}</td></tr>");
                }
                html.Append("</tbody></table>");
                break;

            case "syllabus":
                if (model.SyllabusReports == null || model.SyllabusReports.Count == 0)
                    { html.Append("<p>No data found.</p>"); return; }
                html.Append("<table><thead><tr><th>Title</th><th>Class</th><th>Subject</th><th>Year</th><th>Uploaded By</th><th>Uploaded At</th><th>Active</th></tr></thead><tbody>");
                foreach (var r in model.SyllabusReports)
                {
                    html.Append($"<tr><td>{r.Title}</td><td>{r.ClassName}</td><td>{r.SubjectName}</td><td>{r.AcademicYear}</td><td>{r.UploadedBy}</td><td>{r.UploadedAt:dd MMM yyyy}</td><td>{(r.IsActive?"<span class='badge badge-success'>Yes</span>":"<span class='badge badge-danger'>No</span>")}</td></tr>");
                }
                html.Append("</tbody></table>");
                break;

            case "student-distribution":
                if (model.StudentDistribution == null || model.StudentDistribution.Count == 0)
                    { html.Append("<p>No data found.</p>"); return; }
                html.Append("<table><thead><tr><th>Class</th><th>Section</th><th>Group</th><th>Students</th></tr></thead><tbody>");
                foreach (var r in model.StudentDistribution)
                {
                    html.Append($"<tr><td>{r.ClassName}</td><td>{r.SectionName}</td><td>{r.GroupName}</td><td>{r.StudentCount}</td></tr>");
                }
                html.Append("</tbody></table>");
                break;

            case "capacity":
                if (model.CapacityReports == null || model.CapacityReports.Count == 0)
                    { html.Append("<p>No data found.</p>"); return; }
                html.Append("<table><thead><tr><th>Class</th><th>Capacity</th><th>Occupied</th><th>Available</th><th>Occupancy</th></tr></thead><tbody>");
                foreach (var r in model.CapacityReports)
                {
                    html.Append($"<tr><td>{r.ClassName}</td><td>{r.TotalCapacity}</td><td>{r.TotalOccupied}</td><td>{r.AvailableSeats}</td><td>{r.OccupancyPercent}%</td></tr>");
                }
                html.Append("</tbody></table>");
                break;

            default:
                html.Append("<p>Select a report type.</p>");
                break;
        }
    }

    public async Task<byte[]> ExportExcelAsync(AcademicReportFilterDto filter, CancellationToken ct = default)
    {
        var model = await GetReportAsync(filter, ct);
        var rows = new List<string[]>();

        switch (filter.ReportType)
        {
            case "academic-year":
                rows.Add(new[] { "Name", "Starts On", "Ends On", "Classes", "Sections", "Students", "Subjects", "Status" });
                if (model.AcademicYearReports != null)
                {
                    foreach (var r in model.AcademicYearReports)
                        rows.Add(new[] { r.Name, r.StartsOn.ToString("dd MMM yyyy"), r.EndsOn.ToString("dd MMM yyyy"), r.ClassCount.ToString(), r.SectionCount.ToString(), r.StudentCount.ToString(), r.SubjectCount.ToString(), r.IsActive ? "Active" : "Inactive" });
                }
                return SimpleExcelWriter.WriteWorkbook("AcademicYear", rows);

            case "class":
                rows.Add(new[] { "Name", "Code", "Sections", "Students", "Capacity", "Occupancy%", "Subjects" });
                if (model.ClassReports != null)
                {
                    foreach (var r in model.ClassReports)
                        rows.Add(new[] { r.Name, r.Code, r.SectionCount.ToString(), r.StudentCount.ToString(), r.Capacity.ToString(), r.OccupancyPercent.ToString("F1"), r.SubjectCount.ToString() });
                }
                return SimpleExcelWriter.WriteWorkbook("Classes", rows);

            case "section":
                rows.Add(new[] { "Section", "Class", "Group", "Capacity", "Occupied", "Occupancy%" });
                if (model.SectionReports != null)
                {
                    foreach (var r in model.SectionReports)
                        rows.Add(new[] { r.Name, r.ClassName, r.GroupName, r.Capacity.ToString(), r.Occupied.ToString(), r.OccupancyPercent.ToString("F1") });
                }
                return SimpleExcelWriter.WriteWorkbook("Sections", rows);

            case "subject":
                rows.Add(new[] { "Code", "Name", "Category", "Mandatory", "Practical", "Classes", "Teachers" });
                if (model.SubjectReports != null)
                {
                    foreach (var r in model.SubjectReports)
                        rows.Add(new[] { r.Code, r.Name, r.Category, r.IsMandatory ? "Yes" : "No", r.IsPractical ? "Yes" : "No", r.ClassCount.ToString(), r.TeacherCount.ToString() });
                }
                return SimpleExcelWriter.WriteWorkbook("Subjects", rows);

            case "teacher-load":
                rows.Add(new[] { "Teacher", "Subjects", "Classes", "Sections", "Periods/Week" });
                if (model.TeacherLoadReports != null)
                {
                    foreach (var r in model.TeacherLoadReports)
                        rows.Add(new[] { r.TeacherName, r.SubjectNames, r.AssignedClasses.ToString(), r.AssignedSections.ToString(), r.TotalPeriodsPerWeek.ToString() });
                }
                return SimpleExcelWriter.WriteWorkbook("TeacherLoad", rows);

            case "syllabus":
                rows.Add(new[] { "Title", "Class", "Subject", "Year", "Uploaded By", "Uploaded At", "Active" });
                if (model.SyllabusReports != null)
                {
                    foreach (var r in model.SyllabusReports)
                        rows.Add(new[] { r.Title, r.ClassName, r.SubjectName, r.AcademicYear, r.UploadedBy, r.UploadedAt.ToString("dd MMM yyyy"), r.IsActive ? "Yes" : "No" });
                }
                return SimpleExcelWriter.WriteWorkbook("Syllabus", rows);

            case "student-distribution":
                rows.Add(new[] { "Class", "Section", "Group", "Students" });
                if (model.StudentDistribution != null)
                {
                    foreach (var r in model.StudentDistribution)
                        rows.Add(new[] { r.ClassName, r.SectionName, r.GroupName, r.StudentCount.ToString() });
                }
                return SimpleExcelWriter.WriteWorkbook("StudentDistribution", rows);

            case "capacity":
                rows.Add(new[] { "Class", "Capacity", "Occupied", "Available", "Occupancy%" });
                if (model.CapacityReports != null)
                {
                    foreach (var r in model.CapacityReports)
                        rows.Add(new[] { r.ClassName, r.TotalCapacity.ToString(), r.TotalOccupied.ToString(), r.AvailableSeats.ToString(), r.OccupancyPercent.ToString("F1") });
                }
                return SimpleExcelWriter.WriteWorkbook("Capacity", rows);

            default:
                rows.Add(new[] { "Select a report type." });
                return SimpleExcelWriter.WriteWorkbook("Report", rows);
        }
    }
}
