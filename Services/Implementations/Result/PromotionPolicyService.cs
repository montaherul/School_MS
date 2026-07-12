using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using StudentEntity = SchoolManagementSystem.Models.Entities.Student.Student;

namespace SchoolManagementSystem.Services.Implementations.Result;

/// <summary>
/// Promotion policy service: fully configurable promotion rules.
/// Supports GPA-based, position-based, attendance-based, and combined rules.
/// No hardcoded promotion logic.
/// </summary>
public class PromotionPolicyService : IPromotionPolicyService
{
    private readonly IUnitOfWork _uow;

    public PromotionPolicyService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<PromotionPolicy?> GetPromotionPolicyAsync(int academicYearId, int schoolClassId, CancellationToken ct = default)
    {
        return await _uow.Repository<PromotionPolicy>().QueryNoTracking()
            .Include(p => p.Rules)
            .Where(p => p.AcademicYearId == academicYearId
                && p.SchoolClassId == schoolClassId
                && p.IsActive && !p.IsDeleted)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<PromotionPolicy>> GetAllPromotionPoliciesAsync(int academicYearId, CancellationToken ct = default)
    {
        return await _uow.Repository<PromotionPolicy>().QueryNoTracking()
            .Include(p => p.Rules)
            .Where(p => p.AcademicYearId == academicYearId && !p.IsDeleted)
            .ToListAsync(ct);
    }

    public async Task<PromotionPolicy> CreatePromotionPolicyAsync(PromotionPolicy policy, List<PromotionPolicyRule> rules, CancellationToken ct = default)
    {
        await _uow.Repository<PromotionPolicy>().AddAsync(policy);
        await _uow.SaveChangesAsync();

        foreach (var rule in rules)
        {
            rule.PromotionPolicyId = policy.Id;
            await _uow.Repository<PromotionPolicyRule>().AddAsync(rule);
        }
        await _uow.SaveChangesAsync();

        return policy;
    }

    public async Task<PromotionPolicy> UpdatePromotionPolicyAsync(PromotionPolicy policy, List<PromotionPolicyRule> rules, CancellationToken ct = default)
    {
        var existing = await _uow.Repository<PromotionPolicy>().Query()
            .Include(p => p.Rules)
            .FirstOrDefaultAsync(p => p.Id == policy.Id, ct);

        if (existing == null) throw new ArgumentException("Promotion policy not found");

        existing.Name = policy.Name;
        existing.PrimaryMethod = policy.PrimaryMethod;
        existing.MinimumGpa = policy.MinimumGpa;
        existing.MaxPositionForPromotion = policy.MaxPositionForPromotion;
        existing.TopPercentagePromote = policy.TopPercentagePromote;
        existing.MinimumAttendancePercentage = policy.MinimumAttendancePercentage;
        existing.MinimumPassedSubjects = policy.MinimumPassedSubjects;
        existing.UseCombinedRules = policy.UseCombinedRules;
        existing.CriticalSubjectsJson = policy.CriticalSubjectsJson;
        existing.MaxCriticalSubjectFailures = policy.MaxCriticalSubjectFailures;
        existing.IsActive = policy.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;
        _uow.Repository<PromotionPolicy>().Update(existing);

        var existingRules = await _uow.Repository<PromotionPolicyRule>().Query()
            .Where(r => r.PromotionPolicyId == policy.Id).ToListAsync(ct);
        _uow.Repository<PromotionPolicyRule>().RemoveRange(existingRules);

        foreach (var rule in rules)
        {
            rule.PromotionPolicyId = policy.Id;
            rule.Id = 0;
            await _uow.Repository<PromotionPolicyRule>().AddAsync(rule);
        }

        await _uow.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeletePromotionPolicyAsync(int policyId, CancellationToken ct = default)
    {
        var policy = await _uow.Repository<PromotionPolicy>().GetByIdAsync(policyId);
        if (policy == null) return false;

        policy.IsDeleted = true;
        policy.UpdatedAt = DateTime.UtcNow;
        _uow.Repository<PromotionPolicy>().Update(policy);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<PromotionEligibilityResult> EvaluatePromotionAsync(int studentId, int academicYearId, CancellationToken ct = default)
    {
        var student = await _uow.Repository<StudentEntity>().Query()
            .Include(s => s.Class)
            .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted, ct);

        if (student == null)
            return new PromotionEligibilityResult { StudentId = studentId, IsEligible = false, Reason = "Student not found" };

        var finalResult = await _uow.Repository<FinalResult>().Query()
            .FirstOrDefaultAsync(f => f.StudentId == studentId && f.AcademicYearId == academicYearId, ct);

        var policy = await GetPromotionPolicyAsync(academicYearId, student.ClassId, ct);

        return EvaluateStudent(student, finalResult, policy);
    }

    public async Task<List<PromotionEligibilityResult>> EvaluateClassPromotionAsync(int classId, int academicYearId, CancellationToken ct = default)
    {
        var students = await _uow.Repository<StudentEntity>().Query()
            .Where(s => s.ClassId == classId && !s.IsDeleted)
            .ToListAsync(ct);

        var studentIds = students.Select(s => s.Id).ToList();
        var finalResults = await _uow.Repository<FinalResult>().Query()
            .Where(f => f.AcademicYearId == academicYearId && studentIds.Contains(f.StudentId))
            .ToListAsync(ct);

        var policy = await GetPromotionPolicyAsync(academicYearId, classId, ct);
        var finalResultDict = finalResults.ToDictionary(f => f.StudentId);

        // Pre-load subject results for critical subject evaluation (avoids N+1)
        var subjectResults = await _uow.Repository<StudentSubjectResult>().Query()
            .Where(sr => studentIds.Contains(sr.StudentId))
            .ToListAsync(ct);
        var subjectResultsByStudent = subjectResults
            .GroupBy(sr => sr.StudentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var results = new List<PromotionEligibilityResult>();
        foreach (var student in students)
        {
            finalResultDict.TryGetValue(student.Id, out var fr);
            subjectResultsByStudent.TryGetValue(student.Id, out var srList);
            results.Add(EvaluateStudent(student, fr, policy, srList));
        }

        return results;
    }

    public async Task<PromotionExecutionResult> ExecutePromotionAsync(int classId, int academicYearId, int executedByUserId, CancellationToken ct = default)
    {
        var evaluations = await EvaluateClassPromotionAsync(classId, academicYearId, ct);
        var policy = await GetPromotionPolicyAsync(academicYearId, classId, ct);

        var result = new PromotionExecutionResult
        {
            AcademicYearId = academicYearId,
            SchoolClassId = classId,
            TotalStudents = evaluations.Count
        };

        var schoolClass = await _uow.Repository<SchoolClass>().GetByIdAsync(classId);
        int nextClassId = classId + 1;
        int currentSortOrder = schoolClass?.SortOrder ?? 0;
        var nextClass = await _uow.Repository<SchoolClass>().Query()
            .Where(c => c.SortOrder == currentSortOrder + 1 && !c.IsDeleted)
            .FirstOrDefaultAsync(ct);

        if (nextClass == null)
        {
            result.Errors.Add("Next class not found for promotion target.");
            return result;
        }
        nextClassId = nextClass.Id;

        // Pre-load students, final results, history, and settings before the transaction loop
        var studentIds = evaluations.Select(e => e.StudentId).ToList();
        var studentsDict = await _uow.Repository<StudentEntity>().Query()
            .Where(s => studentIds.Contains(s.Id) && !s.IsDeleted)
            .ToDictionaryAsync(s => s.Id, ct);
        var finalResultsDict = await _uow.Repository<FinalResult>().Query()
            .Where(f => f.AcademicYearId == academicYearId && studentIds.Contains(f.StudentId))
            .ToDictionaryAsync(f => f.StudentId, ct);
        var existingHistories = await _uow.Repository<PromotionHistory>().Query()
            .Where(h => studentIds.Contains(h.StudentId) && h.AcademicYearId == academicYearId)
            .ToDictionaryAsync(h => h.StudentId, ct);

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            // Pre-load settings and target class once for group revalidation
            var promoSettings = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(ct);
            var promoNextClass = await _uow.Repository<SchoolClass>().GetByIdAsync(nextClassId);

            foreach (var eval in evaluations)
            {
                var status = eval.Status;
                var history = new PromotionHistory
                {
                    StudentId = eval.StudentId,
                    FromClassId = classId,
                    ToClassId = nextClassId,
                    AcademicYearId = academicYearId,
                    Status = status,
                    PromotedAt = DateTime.Now,
                    PromotedByUserId = executedByUserId,
                    Remarks = eval.Reason
                };
                await _uow.Repository<PromotionHistory>().AddAsync(history);

                // Update student's class assignment and revalidate group
                if (status == PromotionStatus.Promoted)
                {
                    if (studentsDict.TryGetValue(eval.StudentId, out var student))
                    {
                        student.ClassId = nextClassId;

                        if (promoSettings != null && promoNextClass != null)
                        {
                            bool targetRequiresGroup = promoNextClass.SortOrder >= promoSettings.GroupStartsFromClassId;
                            if (targetRequiresGroup && !student.StudentGroupId.HasValue)
                                throw new InvalidOperationException($"Student {student.FullName} requires an academic group for {promoNextClass.Name}.");
                            if (!targetRequiresGroup)
                                student.StudentGroupId = null;
                        }

                        _uow.Repository<StudentEntity>().Update(student);

                        // Cascade updates to attendance, exam results, and group assignment
                        await RebuildStudentCascadeAsync(
                            student.Id,
                            student.ClassId,
                            student.SectionId,
                            student.StudentGroupId,
                            academicYearId,
                            ct);
                    }
                }

                if (finalResultsDict.TryGetValue(eval.StudentId, out var finalResult))
                {
                    finalResult.PromotionStatus = status;
                    finalResult.PromotionRemarks = eval.Reason;
                    _uow.Repository<FinalResult>().Update(finalResult);
                }

                result.Records.Add(new PromotionRecord
                {
                    StudentId = eval.StudentId,
                    StudentName = eval.StudentName,
                    FromClassId = classId,
                    ToClassId = nextClassId,
                    Status = status,
                    Reason = eval.Reason,
                    ProcessedAt = DateTime.Now,
                    ProcessedByUserId = executedByUserId
                });

                switch (status)
                {
                    case PromotionStatus.Promoted: result.PromotedCount++; break;
                    case PromotionStatus.Repeat: result.RepeatCount++; break;
                    case PromotionStatus.Failed: result.FailedCount++; break;
                }
            }

            var execution = new PromotionExecution
            {
                AcademicYearId = academicYearId,
                SchoolClassId = classId,
                PromotionPolicyId = policy?.Id,
                TotalStudents = result.TotalStudents,
                PromotedCount = result.PromotedCount,
                RepeatCount = result.RepeatCount,
                FailedCount = result.FailedCount,
                ExecutedByUserId = executedByUserId,
                ExecutedAt = DateTime.Now,
                IsApproved = true
            };
            await _uow.Repository<PromotionExecution>().AddAsync(execution);
            await _uow.SaveChangesAsync();
        });

        return result;
    }

    private async Task RebuildStudentCascadeAsync(int studentId, int newClassId, int? newSectionId, int? newGroupId, int academicYearId, CancellationToken ct = default)
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

    public async Task<List<GroupAssignmentResult>> AssignGroupsAsync(int fromClassId, int toClassId, int academicYearId, int? processedByUserId, CancellationToken ct = default)
    {
        var config = await _uow.Repository<GroupPromotionConfig>().Query()
            .FirstOrDefaultAsync(c => c.AcademicYearId == academicYearId
                && c.FromClassId == fromClassId && c.ToClassId == toClassId
                && c.IsActive && !c.IsDeleted, ct);

        var method = config?.AssignmentMethod ?? GroupAssignmentMethod.MeritBased;

        var students = await _uow.Repository<StudentEntity>().Query()
            .Where(s => s.ClassId == fromClassId && !s.IsDeleted)
            .ToListAsync(ct);

        var studentIds = students.Select(s => s.Id).ToList();
        var finalResults = await _uow.Repository<FinalResult>().Query()
            .Where(f => f.AcademicYearId == academicYearId && studentIds.Contains(f.StudentId))
            .ToListAsync(ct);

        var groups = await _uow.Repository<StudentGroup>().Query()
            .Where(g => g.IsActive && !g.IsDeleted)
            .ToListAsync(ct);

        var results = new List<GroupAssignmentResult>();
        var finalResultDict = finalResults.ToDictionary(f => f.StudentId);

        switch (method)
        {
            case GroupAssignmentMethod.MeritBased:
                var ranked = finalResults.OrderByDescending(f => f.FinalGpa).ThenByDescending(f => f.WeightedTotalMarks).ToList();
                var groupSize = groups.Count > 0 ? (int)Math.Ceiling((double)ranked.Count / groups.Count) : 0;
                for (int i = 0; i < ranked.Count; i++)
                {
                    var groupIndex = groupSize > 0 ? i / groupSize : 0;
                    var assignedGroup = groups.Count > groupIndex ? groups[groupIndex] : null;
                    results.Add(new GroupAssignmentResult
                    {
                        StudentId = ranked[i].StudentId,
                        StudentName = "",
                        AssignedGroupId = assignedGroup?.Id,
                        AssignedGroupName = assignedGroup?.Name ?? "",
                        Method = "MeritBased"
                    });
                }
                break;

            case GroupAssignmentMethod.SubjectGpaBased:
                Dictionary<string, decimal> thresholds = [];
                if (!string.IsNullOrEmpty(config?.ConfigurationJson))
                {
                    thresholds = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, decimal>>(config.ConfigurationJson) ?? [];
                }
                foreach (var student in students)
                {
                    finalResultDict.TryGetValue(student.Id, out var fr);
                    var gpa = fr?.FinalGpa ?? 0;
                    string assignedGroupName = "";
                    int? assignedGroupId = null;

                    foreach (var threshold in thresholds.OrderByDescending(t => t.Value))
                    {
                        if (gpa >= threshold.Value)
                        {
                            var grp = groups.FirstOrDefault(g => g.Name.Contains(threshold.Key, StringComparison.OrdinalIgnoreCase));
                            assignedGroupName = grp?.Name ?? threshold.Key;
                            assignedGroupId = grp?.Id;
                            break;
                        }
                    }

                    if (assignedGroupId == null && groups.Count > 0)
                    {
                        assignedGroupName = groups.Last().Name;
                        assignedGroupId = groups.Last().Id;
                    }

                    results.Add(new GroupAssignmentResult
                    {
                        StudentId = student.Id,
                        StudentName = student.FullName,
                        AssignedGroupId = assignedGroupId,
                        AssignedGroupName = assignedGroupName,
                        Method = "SubjectGpaBased"
                    });
                }
                break;

            default:
                foreach (var student in students)
                {
                    results.Add(new GroupAssignmentResult
                    {
                        StudentId = student.Id,
                        StudentName = student.FullName,
                        AssignedGroupId = null,
                        AssignedGroupName = "",
                        Method = method.ToString()
                    });
                }
                break;
        }

        return results;
    }

    private PromotionEligibilityResult EvaluateStudent(StudentEntity student, FinalResult? finalResult, PromotionPolicy? policy, List<StudentSubjectResult>? subjectResults = null)
    {
        var eval = new PromotionEligibilityResult
        {
            StudentId = student.Id,
            StudentName = student.FullName,
            FinalGpa = finalResult?.FinalGpa ?? 0,
            FinalPosition = finalResult?.FinalPosition ?? 0,
            AttendancePercentage = finalResult?.AttendancePercentage ?? 0,
            TotalPassedSubjects = finalResult?.TotalPassedSubjects ?? 0,
            TotalFailedSubjects = finalResult?.TotalFailedSubjects ?? 0
        };

        if (finalResult == null)
        {
            eval.IsEligible = false;
            eval.Reason = "No final result found";
            eval.Status = PromotionStatus.Pending;
            return eval;
        }

        if (policy == null)
        {
            eval.IsEligible = finalResult.FinalGpa >= 1.0m;
            eval.Reason = eval.IsEligible ? "Default promotion (no policy configured)" : "GPA below default threshold";
            eval.Status = eval.IsEligible ? PromotionStatus.Promoted : PromotionStatus.Repeat;
            return eval;
        }

        bool primaryMet = policy.PrimaryMethod switch
        {
            PromotionMethod.GpaBased => finalResult.FinalGpa >= policy.MinimumGpa,
            PromotionMethod.PositionBased => (policy.MaxPositionForPromotion.HasValue && finalResult.FinalPosition <= policy.MaxPositionForPromotion.Value)
                || (policy.TopPercentagePromote.HasValue && finalResult.FinalPosition > 0 && student.ClassId > 0),
            PromotionMethod.AttendanceBased => policy.MinimumAttendancePercentage.HasValue && finalResult.AttendancePercentage >= policy.MinimumAttendancePercentage.Value,
            PromotionMethod.PassedSubjectsBased => policy.MinimumPassedSubjects.HasValue && finalResult.TotalPassedSubjects >= policy.MinimumPassedSubjects.Value,
            _ => finalResult.FinalGpa >= policy.MinimumGpa
        };

        bool criticalSubjectsMet = true;
        if (!string.IsNullOrEmpty(policy.CriticalSubjectsJson))
        {
            var criticalSubjects = System.Text.Json.JsonSerializer.Deserialize<List<string>>(policy.CriticalSubjectsJson) ?? [];
            if (criticalSubjects.Count > 0)
            {
                // Use pre-loaded subject results if available, otherwise load (for single-student evaluation)
                var srList = subjectResults ?? _uow.Repository<StudentSubjectResult>().Query()
                    .Where(sr => sr.StudentId == student.Id && sr.ExamId > 0)
                    .ToList();
                var failedCritical = srList
                    .Where(sr => !sr.IsPassed && criticalSubjects.Any(cs =>
                        sr.SubjectId > 0 && srList.Any(x => x.SubjectId == sr.SubjectId)))
                    .GroupBy(sr => sr.SubjectId).Count();
                criticalSubjectsMet = failedCritical <= policy.MaxCriticalSubjectFailures;
            }
        }

        bool eligible;
        if (policy.UseCombinedRules && policy.Rules.Any())
        {
            eligible = primaryMet && criticalSubjectsMet;
            foreach (var rule in policy.Rules.Where(r => r.IsActive).OrderBy(r => r.DisplayOrder))
            {
                bool ruleMet = EvaluateRule(rule, eval);
                if (rule.LogicalOperator == "OR" && ruleMet) { eligible = true; break; }
                if (rule.LogicalOperator == "AND" && !ruleMet) { eligible = false; break; }
            }
        }
        else
        {
            eligible = primaryMet && criticalSubjectsMet;
        }

        eval.IsEligible = eligible;
        eval.Reason = eligible
            ? $"Meets promotion criteria ({policy.PrimaryMethod})"
            : $"Does not meet {policy.PrimaryMethod} criteria";
        eval.Status = eligible ? PromotionStatus.Promoted : PromotionStatus.Repeat;
        return eval;
    }

    private bool EvaluateRule(PromotionPolicyRule rule, PromotionEligibilityResult eval)
    {
        decimal actualValue = rule.CriterionType.ToLowerInvariant() switch
        {
            "gpa" => eval.FinalGpa,
            "marks" => eval.FinalGpa,
            "position" => eval.FinalPosition,
            "attendance" => eval.AttendancePercentage,
            "passedsubjects" => eval.TotalPassedSubjects,
            "failedsubjects" => eval.TotalFailedSubjects,
            _ => 0
        };

        bool met = rule.Operator.ToLowerInvariant() switch
        {
            "greaterthan" => actualValue > rule.ThresholdValue,
            "lessthan" => actualValue < rule.ThresholdValue,
            "equals" => actualValue == rule.ThresholdValue,
            "greaterthanorequal" => actualValue >= rule.ThresholdValue,
            "lessthanorequal" => actualValue <= rule.ThresholdValue,
            _ => false
        };

        return rule.IsInverse ? !met : met;
    }

    public async Task<PromotionPolicy?> GetPolicyByIdWithRulesAsync(int policyId, CancellationToken ct = default)
    {
        return await _uow.Repository<PromotionPolicy>().Query()
            .Include(p => p.Rules)
            .FirstOrDefaultAsync(p => p.Id == policyId && !p.IsDeleted, ct);
    }

    public async Task<PromotionExecution?> GetPromotionExecutionAsync(int academicYearId, int classId, CancellationToken ct = default)
    {
        return await _uow.Repository<PromotionExecution>().Query()
            .Where(e => e.AcademicYearId == academicYearId && e.SchoolClassId == classId && !e.IsDeleted)
            .OrderByDescending(e => e.ExecutedAt)
            .FirstOrDefaultAsync(ct);
    }
}
