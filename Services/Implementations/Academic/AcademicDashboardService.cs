using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Routine;
using SchoolManagementSystem.Models.Entities.Student;
using EmployeeEntity = SchoolManagementSystem.Models.Entities.Employee.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Routine;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class AcademicDashboardService : IAcademicDashboardService
{
    private readonly IUnitOfWork _uow;
    private readonly ITeacherLoadRepository _teacherLoadRepo;

    public AcademicDashboardService(IUnitOfWork uow, ITeacherLoadRepository teacherLoadRepo)
    {
        _uow = uow;
        _teacherLoadRepo = teacherLoadRepo;
    }

    public async Task<AcademicDashboardDto> GetDashboardAsync(CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var currentYear = today.Year;

        var totalStudents = await _uow.Repository<Student>().Query().AsNoTracking()
            .CountAsync(s => !s.IsDeleted, ct);
        var totalTeachers = await _uow.Repository<EmployeeEntity>().Query().AsNoTracking()
            .CountAsync(e => !e.IsDeleted && e.IsTeachingStaff, ct);
        var totalClasses = await _uow.Repository<SchoolClass>().Query().AsNoTracking()
            .CountAsync(c => !c.IsDeleted, ct);
        var totalSections = await _uow.Repository<Section>().Query().AsNoTracking()
            .CountAsync(s => !s.IsDeleted, ct);
        var totalSubjects = await _uow.Repository<Subject>().Query().AsNoTracking()
            .CountAsync(s => !s.IsDeleted, ct);
        var activeYears = await _uow.Repository<AcademicYear>().Query().AsNoTracking()
            .CountAsync(y => !y.IsDeleted, ct);
        var eventsToday = await _uow.Repository<AcademicCalendar>().Query().AsNoTracking()
            .CountAsync(c => c.Date == today, ct);
        var activeGroups = await _uow.Repository<StudentGroup>().Query().AsNoTracking()
            .CountAsync(g => !g.IsDeleted, ct);
        var totalCapacity = await _uow.Repository<Section>().Query().AsNoTracking()
            .Where(s => !s.IsDeleted)
            .SumAsync(s => (int?)s.Capacity ?? 0, ct);
        var syllabusTotal = await _uow.Repository<Syllabus>().Query().AsNoTracking()
            .CountAsync(s => !s.IsDeleted, ct);
        var syllabusCompleted = await _uow.Repository<Syllabus>().Query().AsNoTracking()
            .CountAsync(s => !s.IsDeleted && !string.IsNullOrEmpty(s.FilePath), ct);
        var todayClasses = await _uow.Repository<AcademicCalendar>().Query().AsNoTracking()
            .CountAsync(c => c.Date == today && c.IsWorkingDay, ct);
        var totalRoutines = await _uow.Repository<RoutineEntry>().Query().AsNoTracking()
            .CountAsync(ct);
        var totalClassrooms = await _uow.Repository<Room>().Query().AsNoTracking()
            .CountAsync(r => !r.IsDeleted, ct);

        var activeAcademicYear = await _uow.Repository<AcademicYear>().Query().AsNoTracking()
            .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct);
        var academicYearId = activeAcademicYear?.Id ?? 0;

        var teacherLoad = academicYearId > 0
            ? await _teacherLoadRepo.GetTeacherLoadSummaryAsync(academicYearId)
            : new List<Models.DTOs.Routine.TeacherLoadDto>();

        var upcomingExams = await _uow.Repository<AcademicCalendar>().Query().AsNoTracking()
            .Where(c => c.Date >= today && c.IsExamDay)
            .OrderBy(c => c.Date)
            .Take(5)
            .Select(c => new UpcomingExamItem
            {
                ExamName = c.Title,
                Date = c.Date,
                Subject = c.Remarks ?? ""
            })
            .ToListAsync(ct);

        var upcomingHolidays = await _uow.Repository<AcademicCalendar>().Query().AsNoTracking()
            .Where(c => c.Date >= today && c.IsHoliday)
            .OrderBy(c => c.Date)
            .Take(5)
            .Select(c => new UpcomingHolidayItem
            {
                Name = c.Title,
                Date = c.Date,
                HolidayType = c.HolidayType ?? ""
            })
            .ToListAsync(ct);

        var monthlyTrend = await _uow.Repository<AcademicCalendar>().Query().AsNoTracking()
            .Where(c => c.Date.Year == currentYear)
            .GroupBy(c => c.Date.Month)
            .Select(g => new MonthlyTrendItem
            {
                Month = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                WorkingDays = g.Count(c => c.IsWorkingDay),
                Holidays = g.Count(c => c.IsHoliday),
                ExamDays = g.Count(c => c.IsExamDay)
            })
            .ToListAsync(ct);

        var studentDist = await _uow.Repository<Student>().Query().AsNoTracking()
            .Where(s => !s.IsDeleted && s.ClassId != null)
            .GroupBy(s => s.ClassId)
            .Select(g => new { ClassId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var classNames = await _uow.Repository<SchoolClass>().Query().AsNoTracking()
            .Where(c => !c.IsDeleted)
            .ToDictionaryAsync(c => c.Id, c => c.Name, ct);

        var subjectCategories = await _uow.Repository<Subject>().Query().AsNoTracking()
            .Where(s => !s.IsDeleted)
            .GroupBy(s => s.Category)
            .Select(g => new SubjectCategoryItem
            {
                Category = string.IsNullOrEmpty(g.Key) ? "General" : g.Key,
                Count = g.Count()
            })
            .ToListAsync(ct);

        var sectionIds = await _uow.Repository<Section>().Query().AsNoTracking()
            .Where(s => !s.IsDeleted)
            .Select(s => s.Id)
            .ToListAsync(ct);

        var occupiedCounts = await _uow.Repository<Student>().Query().AsNoTracking()
            .Where(st => !st.IsDeleted && sectionIds.Contains(st.SectionId))
            .GroupBy(st => st.SectionId)
            .Select(g => new { SectionId = g.Key, Count = g.Count() })
            .ToListAsync(ct);
        var occupiedLookup = occupiedCounts.ToDictionary(x => x.SectionId, x => x.Count);

        var sectionData = await _uow.Repository<Section>().Query().AsNoTracking()
            .Where(s => !s.IsDeleted)
            .Include(s => s.SchoolClass)
            .Select(s => new { s.Id, s.Name, ClassName = s.SchoolClass != null ? s.SchoolClass.Name : "", s.Capacity })
            .ToListAsync(ct);

        var sectionCapacity = sectionData.Select(s => new SectionCapacityItem
        {
            SectionName = s.Name,
            ClassName = s.ClassName,
            Capacity = s.Capacity,
            Occupied = occupiedLookup.GetValueOrDefault(s.Id),
            UtilizationPercent = s.Capacity > 0 ? Math.Round((double)occupiedLookup.GetValueOrDefault(s.Id) / s.Capacity * 100, 1) : 0
        }).ToList();

        var examDist = await _uow.Repository<AcademicCalendar>().Query().AsNoTracking()
            .Where(c => c.Date.Year == currentYear && c.IsExamDay)
            .GroupBy(c => c.Date.Month)
            .Select(g => new ExamDistributionItem
            {
                Month = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                ExamCount = g.Count()
            })
            .ToListAsync(ct);

        var ratio = totalTeachers > 0 ? totalStudents / totalTeachers : 0;
        var utilPercent = totalCapacity > 0 ? Math.Round((double)totalStudents / totalCapacity * 100, 1) : 0;
        var avgLoad = teacherLoad.Count > 0 ? Math.Round(teacherLoad.Average(t => t.TotalPeriodsPerWeek), 1) : 0;

        return new AcademicDashboardDto
        {
            TotalStudents = totalStudents,
            TotalTeachers = totalTeachers,
            TotalClasses = totalClasses,
            TotalSections = totalSections,
            TotalSubjects = totalSubjects,
            ActiveAcademicYears = activeYears,
            CalendarEventsToday = eventsToday,
            StudentTeacherRatio = ratio,
            CapacityUtilizationPercent = utilPercent,
            TotalRoutines = totalRoutines,
            TotalClassrooms = totalClassrooms,
            ActiveGroups = activeGroups,
            SyllabusTotal = syllabusTotal,
            SyllabusCompleted = syllabusCompleted,
            SyllabusPending = syllabusTotal - syllabusCompleted,
            SyllabusCompletionPercent = syllabusTotal > 0 ? Math.Round((double)syllabusCompleted / syllabusTotal * 100, 1) : 0,
            UpcomingExams = upcomingExams.Count,
            UpcomingHolidays = upcomingHolidays.Count,
            TodayClasses = todayClasses,
            TeacherLoadAverage = avgLoad,
            UpcomingExamList = upcomingExams,
            UpcomingHolidayList = upcomingHolidays,
            MonthlyTrend = monthlyTrend,
            StudentDistribution = studentDist.Select(s => new StudentDistributionItem
            {
                ClassName = classNames.GetValueOrDefault(s.ClassId, "Unknown"),
                StudentCount = s.Count
            }).ToList(),
            TeacherWorkload = teacherLoad.Select(t => new TeacherWorkloadItem
            {
                TeacherName = t.TeacherName,
                SubjectCount = t.TotalSubjects,
                ClassCount = t.TotalClasses,
                TotalPeriods = t.TotalPeriodsPerWeek
            }).ToList(),
            SubjectCategories = subjectCategories,
            SectionCapacity = sectionCapacity,
            ExamDistribution = examDist
        };
    }
}
