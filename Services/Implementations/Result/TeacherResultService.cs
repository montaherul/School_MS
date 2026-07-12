using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Notification;
using SchoolManagementSystem.Models.Entities.Routine;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Repositories.Interfaces.Teachers;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Routine;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class TeacherResultService : ITeacherResultService
{
    private readonly IUnitOfWork _uow;
    private readonly ITeacherSubjectAssignmentRepository _subjectAssignmentRepo;
    private readonly IExamRepository _examRepository;
    private readonly IMarkEntryRepository _markEntryRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly ISchoolClassRepository _classRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly IRoutineEntryRepository _routineEntryRepo;
    private readonly IRoutinePeriodRepository _routinePeriodRepo;

    public TeacherResultService(
        IUnitOfWork uow,
        ITeacherSubjectAssignmentRepository subjectAssignmentRepo,
        IExamRepository examRepository,
        IMarkEntryRepository markEntryRepository,
        ISubjectRepository subjectRepository,
        ISchoolClassRepository classRepository,
        ISectionRepository sectionRepository,
        IRoutineEntryRepository routineEntryRepo,
        IRoutinePeriodRepository routinePeriodRepo)
    {
        _uow = uow;
        _subjectAssignmentRepo = subjectAssignmentRepo;
        _examRepository = examRepository;
        _markEntryRepository = markEntryRepository;
        _subjectRepository = subjectRepository;
        _classRepository = classRepository;
        _sectionRepository = sectionRepository;
        _routineEntryRepo = routineEntryRepo;
        _routinePeriodRepo = routinePeriodRepo;
    }

    public async Task<TeacherResultDashboardDto> GetDashboardAsync(int teacherId, int academicYearId, CancellationToken ct = default)
    {
        var dashboard = new TeacherResultDashboardDto
        {
            TeacherId = teacherId,
            TeacherName = ""
        };

        var assignments = await _subjectAssignmentRepo
            .QueryNoTracking()
            .Include(a => a.Subject)
            .Include(a => a.Class)
            .Include(a => a.Section)
            .Include(a => a.Group)
            .Include(a => a.Teacher)
                .ThenInclude(t => t!.Employee)
            .Where(a => a.TeacherId == teacherId && a.AcademicYearId == academicYearId && a.IsActive)
            .ToListAsync(ct);

        if (assignments.Count == 0)
            return dashboard;

        dashboard.TeacherName = assignments[0].Teacher?.Employee?.FullName ?? "";

        var subjectIds = assignments.Select(a => a.SubjectId).Distinct().ToList();
        dashboard.TotalAssignedSubjects = subjectIds.Count;

        var classIds = assignments.Select(a => a.ClassId).Distinct().ToList();
        var sectionIds = assignments.Select(a => a.SectionId).Distinct().ToList();
        var groupIds = assignments.Where(a => a.GroupId.HasValue).Select(a => a.GroupId!.Value).Distinct().ToList();

        var exams = await _examRepository
            .QueryNoTracking()
            .Where(e => e.AcademicYearId == academicYearId && classIds.Contains(e.ClassId) && e.IsLocked == false)
            .ToListAsync(ct);

        var examIds = exams.Select(e => e.Id).ToList();

        var markEntries = await _markEntryRepository
            .QueryNoTracking()
            .Where(m => examIds.Contains(m.ExamId) && subjectIds.Contains(m.SubjectId) && m.EnteredByTeacherId == teacherId)
            .ToListAsync(ct);

        var pendingExams = new List<TeacherPendingExamDto>();

        foreach (var assignment in assignments)
        {
            var matchingExams = exams.Where(e => e.ClassId == assignment.ClassId).ToList();

            foreach (var exam in matchingExams)
            {
                var entries = markEntries
                    .Where(m => m.ExamId == exam.Id && m.SubjectId == assignment.SubjectId && m.ClassId == assignment.ClassId && m.SectionId == assignment.SectionId)
                    .ToList();

                var completedCount = entries.Count(m => m.Status >= ResultWorkflowStatus.Submitted);
                var totalCount = entries.Count;
                var pendingCount = entries.Count(m => m.Status == ResultWorkflowStatus.Draft);

                var status = exam.Status switch
                {
                    ResultWorkflowStatus.Published => "Published",
                    ResultWorkflowStatus.Locked => "Locked",
                    ResultWorkflowStatus.Approved => "Approved",
                    ResultWorkflowStatus.Submitted => "Submitted",
                    _ => "Draft"
                };

                pendingExams.Add(new TeacherPendingExamDto
                {
                    ExamId = exam.Id,
                    ExamName = exam.Name,
                    SubjectId = assignment.SubjectId,
                    SubjectName = assignment.Subject?.Name ?? "",
                    ClassId = assignment.ClassId,
                    ClassName = assignment.Class?.Name ?? "",
                    SectionId = assignment.SectionId,
                    SectionName = assignment.Section?.Name ?? "",
                    GroupId = assignment.GroupId,
                    GroupName = assignment.Group?.Name ?? "",
                    StudentCount = totalCount,
                    CompletedCount = completedCount,
                    Status = status,
                    IsReadOnly = exam.Status == ResultWorkflowStatus.Published || exam.Status == ResultWorkflowStatus.Locked
                });
            }
        }

        dashboard.PendingExams = pendingExams;
        dashboard.TotalAssignedExams = pendingExams.Select(p => p.ExamId).Distinct().Count();
        dashboard.PendingMarkEntries = pendingExams.Sum(p => p.StudentCount - p.CompletedCount);
        dashboard.SubmittedMarkEntries = pendingExams.Sum(p => p.CompletedCount);
        dashboard.TotalMarkEntries = pendingExams.Sum(p => p.StudentCount);
        dashboard.CompletionPercentage = dashboard.TotalMarkEntries > 0
            ? Math.Round((double)dashboard.SubmittedMarkEntries / dashboard.TotalMarkEntries * 100, 1)
            : 0;

        var recentActivity = markEntries
            .Where(m => m.Status >= ResultWorkflowStatus.Submitted)
            .OrderByDescending(m => m.UpdatedAt ?? m.CreatedAt)
            .Take(20)
            .Select(m => new TeacherRecentActivityDto
            {
                Action = m.Status == ResultWorkflowStatus.Submitted ? "Submitted" : "Saved",
                Detail = $"Exam: {exams.FirstOrDefault(e => e.Id == m.ExamId)?.Name ?? "N/A"}, Subject: {assignments.FirstOrDefault(a => a.SubjectId == m.SubjectId)?.Subject?.Name ?? "N/A"}",
                Timestamp = m.UpdatedAt ?? m.CreatedAt
            })
            .ToList();

        dashboard.RecentActivity = recentActivity;

        // ── Today's Schedule ──
        var todayDayNumber = (int)DateTime.UtcNow.AddHours(6).DayOfWeek switch
        {
            0 => 7,  // Sunday → 7 (Friday in BD)
            1 => 1,  // Monday → 1 (Saturday in BD)
            2 => 2,  // Tuesday → 2 (Sunday)
            3 => 3,  // Wednesday → 3 (Monday)
            4 => 4,  // Thursday → 4 (Tuesday)
            5 => 5,  // Friday → 5 (Wednesday)
            6 => 6,  // Saturday → 6 (Thursday)
            _ => 0
        };

        var todayEntries = await _routineEntryRepo.GetTeacherRoutineGridAsync(academicYearId, teacherId, ct);
        var todayFiltered = todayEntries.Where(e => e.DayNumber == todayDayNumber).ToList();

        var periods = await _routinePeriodRepo.GetActivePeriodsAsync(ct);
        var periodLookup = periods.ToDictionary(p => p.Id, p => p);

        dashboard.TodaySchedule = todayFiltered.Select(e => new TodayScheduleItemDto
        {
            EntryId = e.Id,
            PeriodName = e.PeriodName,
            StartTime = periodLookup.TryGetValue(e.RoutinePeriodId, out var p) ? p.StartTime : "",
            EndTime = periodLookup.TryGetValue(e.RoutinePeriodId, out var p2) ? p2.EndTime : "",
            SubjectName = e.SubjectName,
            ClassName = e.ClassName,
            SectionName = e.SectionName ?? "",
            RoomNo = e.RoomNo,
            DayNumber = e.DayNumber
        }).OrderBy(s => s.StartTime).ToList();

        // ── Quick Actions ──
        var actions = new List<QuickActionItemDto>
        {
            new() { Label = "Mark Entry", Url = "/Marks/Index", IconName = "edit", Color = "primary" },
            new() { Label = "View Schedule", Url = "/Routine/MySchedule", IconName = "calendar", Color = "info" },
            new() { Label = "My Subjects", Url = "/TeacherSubjectAssignment/MySubjects", IconName = "book", Color = "success" },
            new() { Label = "View Reports", Url = "/Marks/Reports", IconName = "chart", Color = "warning" }
        };

        if (dashboard.PendingMarkEntries > 0)
        {
            actions.Insert(0, new QuickActionItemDto
            {
                Label = $"{dashboard.PendingMarkEntries} Pending Marks",
                Url = "/Marks/Index",
                IconName = "alert",
                Color = "danger"
            });
        }

        dashboard.QuickActions = actions;

        // ── Notifications ──
        var notificationRepo = _uow.Repository<NotificationMessage>();
        var recentNotifications = await notificationRepo.QueryNoTracking()
            .Where(n => n.UserId == null && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Take(10)
            .ToListAsync(ct);

        dashboard.Notifications = recentNotifications.Select(n => new NotificationItemDto
        {
            Title = n.Title,
            Body = n.Body,
            IsRead = n.IsRead,
            SentAt = n.SentAt ?? n.CreatedAt
        }).ToList();

        return dashboard;
    }
}
