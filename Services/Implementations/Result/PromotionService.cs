using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Repositories.Interfaces.Students;

namespace SchoolManagementSystem.Services.Implementations.Result;

/// <summary>
/// Promotion service implementing Bangladesh education system rules
/// Handles student promotion, repetition, and conditional promotion
/// </summary>
public class PromotionService : IPromotionService
{
    private readonly IUnitOfWork _uow;
    private readonly IFinalResultRepository _finalResultRepository;
    private readonly IPromotionHistoryRepository _promotionHistoryRepository;
    private readonly IStudentRepository _studentRepository;

    public PromotionService(
        IUnitOfWork uow,
        IFinalResultRepository finalResultRepository,
        IPromotionHistoryRepository promotionHistoryRepository,
        IStudentRepository studentRepository)
    {
        _uow = uow;
        _finalResultRepository = finalResultRepository;
        _promotionHistoryRepository = promotionHistoryRepository;
        _studentRepository = studentRepository;
    }

    public async Task<PromotionEligibility> CalculatePromotionEligibilityAsync(int studentId, int academicYearId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null) throw new ArgumentException("Student not found");

        var finalResult = await _finalResultRepository.FirstOrDefaultAsync(fr => fr.StudentId == studentId && fr.AcademicYearId == academicYearId);
        var rules = await GetPromotionRulesAsync(student.ClassId);

        return CalculatePromotionEligibilityInternal(studentId, finalResult, rules);
    }

    private PromotionEligibility CalculatePromotionEligibilityInternal(int studentId, FinalResult? finalResult, PromotionRules rules)
    {
        if (finalResult == null)
        {
            return new PromotionEligibility
            {
                StudentId = studentId,
                IsEligible = false,
                Reason = "No final result found for the academic year",
                RecommendedAction = "Review Required"
            };
        }

        var eligibility = new PromotionEligibility
        {
            StudentId = studentId,
            GPA = finalResult.FinalGpa,
            FailedSubjects = finalResult.TotalFailedSubjects,
            TotalSubjects = 0,
            IsEligible = false,
            Reason = "",
            RecommendedAction = ""
        };

        if (rules.RequireAllSubjectsPass && finalResult.TotalFailedSubjects > 0)
        {
            eligibility.Reason = $"Failed {finalResult.TotalFailedSubjects} subject(s). All subjects must be passed.";
            eligibility.RecommendedAction = "Repeat";
        }
        else if (finalResult.TotalFailedSubjects > rules.MaximumFailedSubjects)
        {
            eligibility.Reason = $"Failed {finalResult.TotalFailedSubjects} subject(s). Maximum allowed failed subjects: {rules.MaximumFailedSubjects}";
            eligibility.RecommendedAction = "Repeat";
        }
        else if (finalResult.FinalGpa < rules.MinimumGPA)
        {
            eligibility.Reason = $"GPA {finalResult.FinalGpa:F2} below minimum required GPA {rules.MinimumGPA:F2}";
            eligibility.RecommendedAction = "Repeat";
        }
        else if (rules.AllowConditionalPromotion &&
                 finalResult.FinalGpa >= rules.ConditionalPromotionGPA &&
                 finalResult.TotalFailedSubjects <= rules.MaximumFailedSubjects)
        {
            eligibility.IsEligible = true;
            eligibility.Reason = $"Eligible for conditional promotion (GPA: {finalResult.FinalGpa:F2}, Failed subjects: {finalResult.TotalFailedSubjects})";
            eligibility.RecommendedAction = "Conditional Promotion";
        }
        else if (finalResult.TotalFailedSubjects == 0 || finalResult.FinalGpa >= rules.MinimumGPA)
        {
            eligibility.IsEligible = true;
            eligibility.Reason = $"Eligible for promotion (GPA: {finalResult.FinalGpa:F2}, Failed subjects: {finalResult.TotalFailedSubjects})";
            eligibility.RecommendedAction = "Promote";
        }
        else
        {
            eligibility.Reason = "Does not meet promotion criteria";
            eligibility.RecommendedAction = "Repeat";
        }

        return eligibility;
    }

    public async Task<PromotionResult> ProcessClassPromotionAsync(int classId, int academicYearId, int processedByUserId)
    {
        var students = await _studentRepository.ListAsync(s => s.ClassId == classId && !s.IsDeleted);

        var studentIds = students.Select(s => s.Id).ToList();
        var finalResults = (await _finalResultRepository.ListAsync(fr => fr.AcademicYearId == academicYearId && studentIds.Contains(fr.StudentId)))
            .ToDictionary(fr => fr.StudentId);

        var rules = await GetPromotionRulesAsync(classId);

        // Resolve next class using SchoolClass.SortOrder
        var currentClass = await _uow.Repository<SchoolClass>().GetByIdAsync(classId);
        var nextClassId = classId + 1; // fallback
        if (currentClass != null)
        {
            var nextClass = await _uow.Repository<SchoolClass>().FirstOrDefaultAsync(c => c.SortOrder == currentClass.SortOrder + 1 && !c.IsDeleted);
            if (nextClass != null)
            {
                nextClassId = nextClass.Id;
            }
        }

        var result = new PromotionResult
        {
            ClassId = classId,
            AcademicYearId = academicYearId,
            TotalStudents = students.Count
        };

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            // Cache for settings/class to avoid N+1 across iterations
            SchoolSetting? cachedSettings = null;
            SchoolClass? cachedNextSchoolClass = null;

            foreach (var student in students)
            {
                finalResults.TryGetValue(student.Id, out var finalResult);
                var eligibility = CalculatePromotionEligibilityInternal(student.Id, finalResult, rules);

                var status = eligibility.RecommendedAction switch
                {
                    "Promote" => PromotionStatus.Promoted,
                    "Conditional Promotion" => PromotionStatus.Promoted,
                    "Repeat" => PromotionStatus.Repeat,
                    _ => PromotionStatus.Pending
                };

                var promotionHistory = new PromotionHistory
                {
                    StudentId = student.Id,
                    FromClassId = classId,
                    ToClassId = nextClassId,
                    AcademicYearId = academicYearId,
                    Status = status,
                    PromotedAt = DateTime.Now,
                    PromotedByUserId = processedByUserId,
                    Remarks = eligibility.Reason
                };

                await _promotionHistoryRepository.AddAsync(promotionHistory);

                // Update student's class assignment to the promoted class
                if (status == PromotionStatus.Promoted && nextClassId != classId)
                {
                    student.ClassId = nextClassId;

                    // Load settings/class once (cached for subsequent iterations to avoid N+1)
                    cachedSettings ??= await _uow.Repository<SchoolSetting>().FirstOrDefaultAsync(s => !s.IsDeleted);
                    cachedNextSchoolClass ??= await _uow.Repository<SchoolClass>().GetByIdAsync(nextClassId);

                    if (cachedSettings != null && cachedNextSchoolClass != null)
                    {
                        bool targetRequiresGroup = cachedNextSchoolClass.SortOrder >= cachedSettings.GroupStartsFromClassId;
                        if (targetRequiresGroup && !student.StudentGroupId.HasValue)
                            throw new InvalidOperationException($"Student {student.FullName} requires an academic group for {cachedNextSchoolClass.Name}.");
                        if (!targetRequiresGroup)
                            student.StudentGroupId = null;
                    }

                    _studentRepository.Update(student);

                    // Cascade updates to attendance, exam results, and group assignment
                    await RebuildStudentCascadeAsync(
                        student.Id,
                        student.ClassId,
                        student.SectionId,
                        student.StudentGroupId,
                        academicYearId,
                        default);
                }

                if (finalResult != null)
                {
                    finalResult.PromotionStatus = status;
                    finalResult.PromotionRemarks = eligibility.Reason;
                    _finalResultRepository.Update(finalResult);
                }

                var record = new PromotionRecord
                {
                    StudentId = student.Id,
                    StudentName = student.FullName,
                    FromClassId = classId,
                    ToClassId = nextClassId,
                    Status = status,
                    Reason = eligibility.Reason,
                    ProcessedAt = DateTime.Now,
                    ProcessedByUserId = processedByUserId
                };

                result.Records.Add(record);

                switch (status)
                {
                    case PromotionStatus.Promoted:
                        result.PromotedCount++;
                        break;
                    case PromotionStatus.Repeat:
                        result.RepeatCount++;
                        break;
                    default:
                        result.ConditionalCount++;
                        break;
                }
            }

            await _uow.SaveChangesAsync();
        });

        return result;
    }

    public async Task<BulkPromotionResult> BulkPromotionAsync(BulkPromotionRequest request)
    {
        var result = new BulkPromotionResult();

        var students = await _studentRepository.ListAsync(s => s.ClassId == request.FromClassId && !s.IsDeleted);

        var studentIds = students.Select(s => s.Id).ToList();
        var finalResults = (await _finalResultRepository.ListAsync(fr => fr.AcademicYearId == request.AcademicYearId && studentIds.Contains(fr.StudentId)))
            .ToDictionary(fr => fr.StudentId);

        var rules = await GetPromotionRulesAsync(request.FromClassId);

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            // Cache for settings/class to avoid N+1 across iterations
            SchoolSetting? cachedSettings = null;
            SchoolClass? cachedNextSchoolClass = null;

            foreach (var student in students)
            {
                finalResults.TryGetValue(student.Id, out var finalResult);
                var eligibility = CalculatePromotionEligibilityInternal(student.Id, finalResult, rules);

                if (!eligibility.IsEligible && !request.OverrideEligibility)
                {
                    result.FailureCount++;
                    result.Errors.Add($"Student {student.FullName}: {eligibility.Reason}");
                    continue;
                }

                var promotionHistory = new PromotionHistory
                {
                    StudentId = student.Id,
                    FromClassId = request.FromClassId,
                    ToClassId = request.ToClassId,
                    AcademicYearId = request.AcademicYearId,
                    Status = PromotionStatus.Promoted,
                    PromotedAt = DateTime.Now,
                    PromotedByUserId = request.ProcessedByUserId,
                    Remarks = request.Comments
                };

                await _promotionHistoryRepository.AddAsync(promotionHistory);

                // Update student's class assignment to the target class
                if (request.ToClassId != request.FromClassId)
                {
                    student.ClassId = request.ToClassId;

                    // Load settings/class once (cached for subsequent iterations to avoid N+1)
                    cachedSettings ??= await _uow.Repository<SchoolSetting>().FirstOrDefaultAsync(s => !s.IsDeleted);
                    cachedNextSchoolClass ??= await _uow.Repository<SchoolClass>().GetByIdAsync(request.ToClassId);

                    if (cachedSettings != null && cachedNextSchoolClass != null)
                    {
                        bool targetRequiresGroup = cachedNextSchoolClass.SortOrder >= cachedSettings.GroupStartsFromClassId;
                        if (targetRequiresGroup && !student.StudentGroupId.HasValue)
                            throw new InvalidOperationException($"Student {student.FullName} requires an academic group for {cachedNextSchoolClass.Name}.");
                        if (!targetRequiresGroup)
                            student.StudentGroupId = null;
                    }

                    _studentRepository.Update(student);

                    // Cascade updates to attendance, exam results, and group assignment
                    await RebuildStudentCascadeAsync(
                        student.Id,
                        student.ClassId,
                        student.SectionId,
                        student.StudentGroupId,
                        request.AcademicYearId,
                        default);
                }

                if (finalResult != null)
                {
                    finalResult.PromotionStatus = PromotionStatus.Promoted;
                    finalResult.PromotionRemarks = request.Comments;
                    _finalResultRepository.Update(finalResult);
                }

                var record = new PromotionRecord
                {
                    StudentId = student.Id,
                    StudentName = student.FullName,
                    FromClassId = request.FromClassId,
                    ToClassId = request.ToClassId,
                    Status = PromotionStatus.Promoted,
                    Reason = request.Comments,
                    ProcessedAt = DateTime.Now,
                    ProcessedByUserId = request.ProcessedByUserId
                };

                result.SuccessfulPromotions.Add(record);
                result.SuccessCount++;
            }

            await _uow.SaveChangesAsync();
        });

        return result;
    }

    public async Task<PromotionRules> GetPromotionRulesAsync(int classId)
    {
        // Check for stored override rules first
        var stored = await _uow.Repository<ClassPromotionRule>()
            .FirstOrDefaultAsync(r => r.ClassId == classId && r.IsActive && !r.IsDeleted);

        if (stored != null)
        {
            return new PromotionRules
            {
                ClassId = classId,
                MinimumGPA = stored.MinimumGPA,
                MaximumFailedSubjects = stored.MaximumFailedSubjects,
                AllowConditionalPromotion = stored.AllowConditionalPromotion,
                ConditionalPromotionGPA = stored.ConditionalPromotionGPA,
                RequireAllSubjectsPass = stored.RequireAllSubjectsPass,
                CriticalSubjects = stored.CriticalSubjectsJson != null
                    ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(stored.CriticalSubjectsJson) ?? []
                    : []
            };
        }

        // Fall back to computed defaults based on class
        var schoolClass = await _uow.Repository<SchoolClass>().GetByIdAsync(classId);
        var rules = new PromotionRules
        {
            ClassId = classId,
            MinimumGPA = 1.0m,
            MaximumFailedSubjects = 2,
            AllowConditionalPromotion = true,
            ConditionalPromotionGPA = 0.8m,
            RequireAllSubjectsPass = schoolClass?.IsGroupBased ?? false,
            CriticalSubjects = ["Bangla", "English", "Mathematics"]
        };

        var sortOrder = schoolClass?.SortOrder ?? classId;
        if (sortOrder <= 5)
        {
            rules.MaximumFailedSubjects = 3;
            rules.MinimumGPA = 0.5m;
        }
        else if (sortOrder <= 8)
        {
            rules.MaximumFailedSubjects = 2;
            rules.MinimumGPA = 1.0m;
        }
        else
        {
            rules.MaximumFailedSubjects = 1;
            rules.MinimumGPA = 1.5m;
            rules.RequireAllSubjectsPass = true;
        }

        return rules;
    }

    public async Task UpdatePromotionRulesAsync(int classId, PromotionRules rules)
    {
        var existing = await _uow.Repository<ClassPromotionRule>()
            .FirstOrDefaultAsync(r => r.ClassId == classId && r.IsActive && !r.IsDeleted);

        if (existing != null)
        {
            existing.MinimumGPA = rules.MinimumGPA;
            existing.MaximumFailedSubjects = rules.MaximumFailedSubjects;
            existing.AllowConditionalPromotion = rules.AllowConditionalPromotion;
            existing.ConditionalPromotionGPA = rules.ConditionalPromotionGPA;
            existing.RequireAllSubjectsPass = rules.RequireAllSubjectsPass;
            existing.CriticalSubjectsJson = rules.CriticalSubjects != null
                ? System.Text.Json.JsonSerializer.Serialize(rules.CriticalSubjects)
                : null;
            existing.UpdatedAt = DateTime.UtcNow;
            _uow.Repository<ClassPromotionRule>().Update(existing);
        }
        else
        {
            var entity = new ClassPromotionRule
            {
                ClassId = classId,
                MinimumGPA = rules.MinimumGPA,
                MaximumFailedSubjects = rules.MaximumFailedSubjects,
                AllowConditionalPromotion = rules.AllowConditionalPromotion,
                ConditionalPromotionGPA = rules.ConditionalPromotionGPA,
                RequireAllSubjectsPass = rules.RequireAllSubjectsPass,
                CriticalSubjectsJson = rules.CriticalSubjects != null
                    ? System.Text.Json.JsonSerializer.Serialize(rules.CriticalSubjects)
                    : null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Repository<ClassPromotionRule>().AddAsync(entity);
        }

        await _uow.SaveChangesAsync();
    }

    public async Task<IEnumerable<PromotionRecord>> GetStudentPromotionHistoryAsync(int studentId)
    {
        return await _promotionHistoryRepository.Query()
            .Include(p => p.FromClass)
            .Include(p => p.ToClass)
            .Include(p => p.AcademicYear)
            .Where(p => p.StudentId == studentId)
            .OrderByDescending(p => p.PromotedAt)
            .Select(p => new PromotionRecord
            {
                StudentId = p.StudentId,
                StudentName = "",
                FromClassId = p.FromClassId,
                ToClassId = p.ToClassId,
                Status = p.Status,
                Reason = p.Remarks ?? "",
                ProcessedAt = p.PromotedAt,
                ProcessedByUserId = p.PromotedByUserId ?? 0
            })
            .ToListAsync();
    }

    public async Task RebuildStudentCascadeAsync(int studentId, int newClassId, int? newSectionId, int? newGroupId, int academicYearId, CancellationToken ct = default)
    {
        // 1. Batch-update AttendanceRecord
        await _uow.Repository<AttendanceRecord>().Query()
            .Where(a => a.StudentId == studentId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.SchoolClassId, newClassId)
                .SetProperty(a => a.SectionId, a => newSectionId ?? a.SectionId), ct);

        // 2. Batch-update StudentExamResult
        await _uow.Repository<StudentExamResult>().Query()
            .Where(r => r.StudentId == studentId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(r => r.ClassId, newClassId)
                .SetProperty(r => r.SectionId, r => newSectionId ?? r.SectionId)
                .SetProperty(r => r.StudentGroupId, newGroupId), ct);

        // 3. Batch-update StudentSubjectResult
        await _uow.Repository<StudentSubjectResult>().Query()
            .Where(r => r.StudentId == studentId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(r => r.ClassId, newClassId)
                .SetProperty(r => r.SectionId, r => newSectionId ?? r.SectionId)
                .SetProperty(r => r.StudentGroupId, newGroupId), ct);

        // 4. Batch-update FinalResult
        await _uow.Repository<FinalResult>().Query()
            .Where(r => r.StudentId == studentId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(r => r.SchoolClassId, newClassId)
                .SetProperty(r => r.SectionId, r => newSectionId ?? r.SectionId)
                .SetProperty(r => r.StudentGroupId, newGroupId), ct);

        // 5. Handle StudentGroupAssignment
        if (newGroupId.HasValue)
        {
            var existing = await _uow.Repository<StudentGroupAssignment>()
                .FirstOrDefaultAsync(a => a.StudentId == studentId && a.SchoolClassId == newClassId && a.AcademicYearId == academicYearId, ct);

            if (existing != null)
            {
                if (existing.StudentGroupId != newGroupId.Value)
                {
                    existing.StudentGroupId = newGroupId.Value;
                    _uow.Repository<StudentGroupAssignment>().Update(existing);
                }
            }
            else
            {
                var newAssignment = new StudentGroupAssignment
                {
                    StudentId = studentId,
                    StudentGroupId = newGroupId.Value,
                    SchoolClassId = newClassId,
                    AcademicYearId = academicYearId,
                    AssignedDate = DateTime.Now
                };
                await _uow.Repository<StudentGroupAssignment>().AddAsync(newAssignment, ct);
            }
        }
        else
        {
            var existing = await _uow.Repository<StudentGroupAssignment>()
                .FirstOrDefaultAsync(a => a.StudentId == studentId && a.SchoolClassId == newClassId && a.AcademicYearId == academicYearId, ct);
            if (existing != null)
            {
                _uow.Repository<StudentGroupAssignment>().Remove(existing);
            }
        }
    }

    public async Task ReversePromotionAsync(int promotionHistoryId, int reversedByUserId, string reason)
    {
        var promotion = await _promotionHistoryRepository.GetByIdAsync(promotionHistoryId);
        if (promotion == null) throw new ArgumentException("Promotion history not found");

        var student = await _studentRepository.GetByIdAsync(promotion.StudentId);
        if (student == null || student.IsDeleted)
            throw new ArgumentException("Student not found or has been deleted");

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var reversal = new PromotionHistory
            {
                StudentId = promotion.StudentId,
                FromClassId = promotion.ToClassId,
                ToClassId = promotion.FromClassId,
                AcademicYearId = promotion.AcademicYearId,
                Status = PromotionStatus.Repeat,
                PromotedAt = DateTime.Now,
                PromotedByUserId = reversedByUserId,
                Remarks = $"Reversal: {reason}"
            };

            await _promotionHistoryRepository.AddAsync(reversal);

            // Restore student's original class assignment
            if (promotion.FromClassId != student.ClassId)
            {
                student.ClassId = promotion.FromClassId;
                _studentRepository.Update(student);

                // Cascade: revert ClassId in related tables, keep section/group unchanged
                await RebuildStudentCascadeAsync(
                    student.Id,
                    promotion.FromClassId,
                    null,
                    null,
                    promotion.AcademicYearId,
                    default);
            }

            var finalResult = await _finalResultRepository.FirstOrDefaultAsync(fr =>
                fr.StudentId == promotion.StudentId && fr.AcademicYearId == promotion.AcademicYearId);
            if (finalResult != null)
            {
                finalResult.PromotionStatus = PromotionStatus.Repeat;
                finalResult.PromotionRemarks = $"Reversal: {reason}";
                _finalResultRepository.Update(finalResult);
            }

            await _uow.SaveChangesAsync();
        });
    }
}
