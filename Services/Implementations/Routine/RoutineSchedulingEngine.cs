using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Routine;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using RoutineEnt = SchoolManagementSystem.Models.Entities.Routine;

namespace SchoolManagementSystem.Services.Implementations.Routine;

public class RoutineSchedulingEngine
{
    private readonly IUnitOfWork _unitOfWork;

    public RoutineSchedulingEngine(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<RoutineGenerationResult> GenerateAsync(int academicYearId, string createdBy, CancellationToken ct = default)
    {
        var result = new RoutineGenerationResult();
        var startedAt = DateTime.UtcNow;

        try
        {
            var requirements = await _unitOfWork.Repository<SubjectRequirement>()
                .ListAsync(r => r.AcademicYearId == academicYearId && r.PeriodsPerWeek > 0, ct);
            var workingDays = await _unitOfWork.Repository<WorkingDay>()
                .ListAsync(d => d.AcademicYearId == academicYearId && d.IsWorkingDay, ct);
            var periods = await _unitOfWork.Repository<RoutinePeriod>()
                .ListAsync(p => p.IsActive && !p.IsBreak, ct);
            var rooms = await _unitOfWork.Repository<RoutineEnt.Room>()
                .ListAsync(r => r.IsActive, ct);
            var teacherAvailability = await _unitOfWork.Repository<TeacherAvailability>()
                .ListAsync(null, ct);
            var existingEntries = await _unitOfWork.Repository<RoutineEntry>()
                .ListAsync(e => e.AcademicYearId == academicYearId && !e.IsDeleted, ct);

            var workingDayNumbers = workingDays.Select(d => d.DayNumber).ToHashSet();
            var periodNumbers = periods.ToDictionary(p => p.Id, p => p.PeriodNumber);
            var periodLookup = periods.ToDictionary(p => p.Id);

            var availabilityLookup = new HashSet<(int TeacherId, int DayNumber, int RoutinePeriodId)>();
            foreach (var ta in teacherAvailability.Where(a => a.IsAvailable))
                availabilityLookup.Add((ta.TeacherId, ta.DayNumber, ta.RoutinePeriodId));

            var teacherSlots = new Dictionary<(int Day, int PeriodId), int>();
            var roomSlots = new Dictionary<(int Day, int PeriodId), int>();
            var studentSlots = new Dictionary<(int ClassId, int? SectionId, int? GroupId, int Day, int PeriodId), bool>();
            var teacherDaySchedules = new Dictionary<int, List<(int Day, int PeriodNumber)>>();

            foreach (var entry in existingEntries)
            {
                var key = (entry.DayNumber, entry.RoutinePeriodId);
                teacherSlots[key] = entry.TeacherId;
                roomSlots[key] = entry.RoomId;
                studentSlots[(entry.ClassId, entry.SectionId, entry.GroupId, entry.DayNumber, entry.RoutinePeriodId)] = true;

                if (!teacherDaySchedules.ContainsKey(entry.TeacherId))
                    teacherDaySchedules[entry.TeacherId] = [];
                if (periodNumbers.TryGetValue(entry.RoutinePeriodId, out var pn))
                    teacherDaySchedules[entry.TeacherId].Add((entry.DayNumber, pn));
            }

            var tokens = GenerateTokens(requirements);
            result.TotalTokens = tokens.Count;

            var placedEntries = new List<RoutineEntry>();
            var unresolvableTeacherSubjects = new HashSet<(int TeacherId, int SubjectId, int ClassId)>();

            var sessionTeacherSlots = new Dictionary<(int Day, int PeriodId), int>(teacherSlots);
            var sessionRoomSlots = new Dictionary<(int Day, int PeriodId), int>(roomSlots);
            var sessionStudentSlots = new Dictionary<(int ClassId, int? SectionId, int? GroupId, int Day, int PeriodId), bool>(studentSlots);
            var sessionTeacherDaySchedules = teacherDaySchedules.ToDictionary(kvp => kvp.Key, kvp => new List<(int, int)>(kvp.Value));

            int i = 0;
            while (i < tokens.Count)
            {
                var token = tokens[i];
                var isDouble = token.RequiresDoublePeriod && i + 1 < tokens.Count
                    && tokens[i + 1].SubjectRequirementId == token.SubjectRequirementId;

                if (isDouble)
                {
                    var slot = FindBestConsecutiveSlot(
                        token, workingDayNumbers, periods, periodNumbers, periodLookup, rooms,
                        sessionTeacherSlots, sessionRoomSlots, sessionStudentSlots,
                        sessionTeacherDaySchedules, availabilityLookup);

                    if (slot.HasValue)
                    {
                        var (day, firstPeriodId, secondPeriodId, roomId) = slot.Value;
                        var firstPeriod = periodLookup[firstPeriodId];

                        placedEntries.Add(new RoutineEntry
                        {
                            AcademicYearId = academicYearId, ClassId = token.ClassId,
                            SectionId = token.SectionId, GroupId = token.GroupId,
                            SubjectId = token.SubjectId, TeacherId = token.TeacherId,
                            RoomId = roomId, RoutinePeriodId = firstPeriodId,
                            DayNumber = day, IsLab = token.RequiresLab,
                            CreatedBy = createdBy, CreatedAt = DateTime.UtcNow
                        });
                        placedEntries.Add(new RoutineEntry
                        {
                            AcademicYearId = academicYearId, ClassId = token.ClassId,
                            SectionId = token.SectionId, GroupId = token.GroupId,
                            SubjectId = token.SubjectId, TeacherId = token.TeacherId,
                            RoomId = roomId, RoutinePeriodId = secondPeriodId,
                            DayNumber = day, IsLab = token.RequiresLab,
                            CreatedBy = createdBy, CreatedAt = DateTime.UtcNow
                        });

                        result.PlacedTokens += 2;

                        var k1 = (day, firstPeriodId);
                        sessionTeacherSlots[k1] = token.TeacherId;
                        sessionRoomSlots[k1] = roomId;
                        sessionStudentSlots[(token.ClassId, token.SectionId, token.GroupId, day, firstPeriodId)] = true;
                        if (!sessionTeacherDaySchedules.ContainsKey(token.TeacherId))
                            sessionTeacherDaySchedules[token.TeacherId] = [];
                        sessionTeacherDaySchedules[token.TeacherId].Add((day, firstPeriod.PeriodNumber));
                        sessionTeacherDaySchedules[token.TeacherId].Add((day, firstPeriod.PeriodNumber + 1));

                        var k2 = (day, secondPeriodId);
                        sessionTeacherSlots[k2] = token.TeacherId;
                        sessionRoomSlots[k2] = roomId;
                        sessionStudentSlots[(token.ClassId, token.SectionId, token.GroupId, day, secondPeriodId)] = true;

                        i += 2;
                    }
                    else
                    {
                        unresolvableTeacherSubjects.Add((token.TeacherId, token.SubjectId, token.ClassId));
                        result.ConflictTokens += 2;
                        result.Conflicts.Add(new RoutineConflictInfo
                        {
                            ConflictType = "NoDoubleSlot",
                            Description = $"No consecutive double slot for SubjectId={token.SubjectId}, ClassId={token.ClassId}, TeacherId={token.TeacherId}",
                            TeacherId = token.TeacherId, SubjectId = token.SubjectId, ClassId = token.ClassId
                        });
                        i += 2;
                    }
                }
                else
                {
                    var slot = FindBestSlot(
                        token, workingDayNumbers, periods, periodNumbers, periodLookup, rooms,
                        sessionTeacherSlots, sessionRoomSlots, sessionStudentSlots,
                        sessionTeacherDaySchedules, availabilityLookup);

                    if (slot.HasValue)
                    {
                        var (day, periodId, roomId) = slot.Value;
                        var period = periodLookup[periodId];

                        placedEntries.Add(new RoutineEntry
                        {
                            AcademicYearId = academicYearId, ClassId = token.ClassId,
                            SectionId = token.SectionId, GroupId = token.GroupId,
                            SubjectId = token.SubjectId, TeacherId = token.TeacherId,
                            RoomId = roomId, RoutinePeriodId = periodId,
                            DayNumber = day, IsLab = token.RequiresLab,
                            CreatedBy = createdBy, CreatedAt = DateTime.UtcNow
                        });

                        result.PlacedTokens++;

                        var key = (day, periodId);
                        sessionTeacherSlots[key] = token.TeacherId;
                        sessionRoomSlots[key] = roomId;
                        sessionStudentSlots[(token.ClassId, token.SectionId, token.GroupId, day, periodId)] = true;

                        if (!sessionTeacherDaySchedules.ContainsKey(token.TeacherId))
                            sessionTeacherDaySchedules[token.TeacherId] = [];
                        sessionTeacherDaySchedules[token.TeacherId].Add((day, period.PeriodNumber));

                        i++;
                    }
                    else
                    {
                        unresolvableTeacherSubjects.Add((token.TeacherId, token.SubjectId, token.ClassId));
                        result.ConflictTokens++;
                        result.Conflicts.Add(new RoutineConflictInfo
                        {
                            ConflictType = "NoAvailableSlot",
                            Description = $"No valid slot for SubjectId={token.SubjectId}, ClassId={token.ClassId}, TeacherId={token.TeacherId}",
                            TeacherId = token.TeacherId, SubjectId = token.SubjectId, ClassId = token.ClassId
                        });
                        i++;
                    }
                }
            }

            if (result.ConflictTokens > 0)
            {
                await ResolveConflictsAsync(result, tokens, placedEntries,
                    sessionTeacherSlots, sessionRoomSlots, sessionStudentSlots, sessionTeacherDaySchedules,
                    workingDayNumbers, periods, periodNumbers, periodLookup, rooms, availabilityLookup,
                    unresolvableTeacherSubjects, academicYearId, createdBy);
            }

            var generation = new RoutineGeneration
            {
                AcademicYearId = academicYearId,
                Status = result.Success ? "Completed" : "CompletedWithConflicts",
                StartedAt = startedAt,
                CompletedAt = DateTime.UtcNow,
                TotalAssignments = result.TotalTokens,
                SuccessfulAssignments = result.PlacedTokens,
                FailedAssignments = result.ConflictTokens,
                ConflictsDetected = result.Conflicts.Count,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<RoutineGeneration>().AddAsync(generation, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            result.GenerationId = generation.Id;

            var existingToDelete = await _unitOfWork.Repository<RoutineEntry>()
                .ListAsync(e => e.AcademicYearId == academicYearId && !e.IsDeleted, ct);

            foreach (var entry in existingToDelete)
            {
                entry.IsDeleted = true;
                entry.UpdatedBy = createdBy;
                entry.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<RoutineEntry>().Update(entry);
            }
            await _unitOfWork.SaveChangesAsync(ct);

            foreach (var entry in placedEntries)
                entry.GenerationId = generation.Id;

            await _unitOfWork.Repository<RoutineEntry>().AddRangeAsync(placedEntries, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            if (result.Conflicts.Count > 0)
            {
                var conflictEntities = result.Conflicts.Select(c => new RoutineConflict
                {
                    GenerationId = generation.Id,
                    ConflictType = c.ConflictType?.Length > 50 ? c.ConflictType[..50] : c.ConflictType ?? "",
                    Description = c.Description?.Length > 500 ? c.Description[..500] : c.Description ?? "",
                    TeacherId = c.TeacherId,
                    RoomId = c.RoomId,
                    SubjectId = c.SubjectId,
                    ClassId = c.ClassId,
                    RoutinePeriodId = c.RoutinePeriodId,
                    DayNumber = c.DayNumber,
                    IsResolved = false,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _unitOfWork.Repository<RoutineConflict>().AddRangeAsync(conflictEntities, ct);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            return result;
        }
        catch (Exception ex)
        {
            var generation = new RoutineGeneration
            {
                AcademicYearId = academicYearId,
                Status = "Failed",
                StartedAt = startedAt,
                CompletedAt = DateTime.UtcNow,
                TotalAssignments = result.TotalTokens,
                SuccessfulAssignments = 0,
                FailedAssignments = result.TotalTokens,
                ErrorMessage = ex.Message?.Length > 4000 ? ex.Message[..4000] : ex.Message,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<RoutineGeneration>().AddAsync(generation, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            result.GenerationId = generation.Id;
            throw;
        }
    }

    private static List<PlacementToken> GenerateTokens(IReadOnlyList<SubjectRequirement> requirements)
    {
        var tokens = new List<PlacementToken>();

        foreach (var req in requirements)
        {
            for (int i = 0; i < req.PeriodsPerWeek; i++)
            {
                tokens.Add(new PlacementToken(
                    req.Id,
                    req.ClassId,
                    req.SectionId,
                    req.GroupId,
                    req.SubjectId,
                    req.TeacherId,
                    req.RequiresLab,
                    req.RequiresDoublePeriod,
                    req.Priority,
                    req.MaxConsecutive > 0 ? req.MaxConsecutive : 2
                ));
            }
        }

        return tokens
            .OrderByDescending(t => t.Priority)
            .ThenByDescending(t => t.RequiresLab ? 1 : 0)
            .ThenByDescending(t => t.RequiresDoublePeriod ? 1 : 0)
            .ToList();
    }

    private static (int Day, int PeriodId, int RoomId)? FindBestSlot(
        PlacementToken token,
        HashSet<int> workingDayNumbers,
        IReadOnlyList<RoutinePeriod> periods,
        Dictionary<int, int> periodNumbers,
        Dictionary<int, RoutinePeriod> periodLookup,
        IReadOnlyList<RoutineEnt.Room> rooms,
        Dictionary<(int Day, int PeriodId), int> teacherSlots,
        Dictionary<(int Day, int PeriodId), int> roomSlots,
        Dictionary<(int ClassId, int? SectionId, int? GroupId, int Day, int PeriodId), bool> studentSlots,
        Dictionary<int, List<(int Day, int PeriodNumber)>> teacherDaySchedules,
        HashSet<(int TeacherId, int DayNumber, int RoutinePeriodId)> availabilityLookup)
    {
        var eligibleRooms = token.RequiresLab
            ? rooms.Where(r => r.IsLab).ToList()
            : rooms.ToList();

        if (eligibleRooms.Count == 0)
            return null;

        var candidates = new List<(int Day, int PeriodId, int RoomId, int Score)>();

        foreach (var day in workingDayNumbers)
        {
            foreach (var period in periods)
            {
                if (teacherSlots.ContainsKey((day, period.Id)))
                    continue;
                if (roomSlots.ContainsKey((day, period.Id)))
                    continue;
                if (studentSlots.ContainsKey((token.ClassId, token.SectionId, token.GroupId, day, period.Id)))
                    continue;
                if (availabilityLookup.Count > 0 && !availabilityLookup.Contains((token.TeacherId, day, period.Id)))
                    continue;
                if (!CheckConsecutiveLimit(teacherDaySchedules, token.TeacherId, day, period.PeriodNumber, token.MaxConsecutive))
                    continue;

                foreach (var room in eligibleRooms)
                {
                    var score = ScoreSlot(token, day, period, periods, teacherDaySchedules, teacherSlots);
                    candidates.Add((day, period.Id, room.Id, score));
                }
            }
        }

        if (candidates.Count == 0)
            return null;

        var best = candidates.OrderByDescending(c => c.Score).First();
        return (best.Day, best.PeriodId, best.RoomId);
    }

    private static (int Day, int FirstPeriodId, int SecondPeriodId, int RoomId)? FindBestConsecutiveSlot(
        PlacementToken token,
        HashSet<int> workingDayNumbers,
        IReadOnlyList<RoutinePeriod> periods,
        Dictionary<int, int> periodNumbers,
        Dictionary<int, RoutinePeriod> periodLookup,
        IReadOnlyList<RoutineEnt.Room> rooms,
        Dictionary<(int Day, int PeriodId), int> teacherSlots,
        Dictionary<(int Day, int PeriodId), int> roomSlots,
        Dictionary<(int ClassId, int? SectionId, int? GroupId, int Day, int PeriodId), bool> studentSlots,
        Dictionary<int, List<(int Day, int PeriodNumber)>> teacherDaySchedules,
        HashSet<(int TeacherId, int DayNumber, int RoutinePeriodId)> availabilityLookup)
    {
        var eligibleRooms = token.RequiresLab
            ? rooms.Where(r => r.IsLab).ToList()
            : rooms.ToList();

        if (eligibleRooms.Count == 0)
            return null;

        var candidates = new List<(int Day, int FirstPeriodId, int SecondPeriodId, int RoomId, int Score)>();

        foreach (var day in workingDayNumbers)
        {
            var sortedPeriods = periods.OrderBy(p => p.PeriodNumber).ToList();

            for (int pi = 0; pi < sortedPeriods.Count - 1; pi++)
            {
                var first = sortedPeriods[pi];
                var second = sortedPeriods[pi + 1];

                if (second.PeriodNumber != first.PeriodNumber + 1)
                    continue;

                if (teacherSlots.ContainsKey((day, first.Id)))
                    continue;
                if (teacherSlots.ContainsKey((day, second.Id)))
                    continue;
                if (roomSlots.ContainsKey((day, first.Id)))
                    continue;
                if (roomSlots.ContainsKey((day, second.Id)))
                    continue;
                if (studentSlots.ContainsKey((token.ClassId, token.SectionId, token.GroupId, day, first.Id)))
                    continue;
                if (studentSlots.ContainsKey((token.ClassId, token.SectionId, token.GroupId, day, second.Id)))
                    continue;
                if (availabilityLookup.Count > 0)
                {
                    if (!availabilityLookup.Contains((token.TeacherId, day, first.Id)))
                        continue;
                    if (!availabilityLookup.Contains((token.TeacherId, day, second.Id)))
                        continue;
                }
                if (!CheckConsecutiveLimit(teacherDaySchedules, token.TeacherId, day, first.PeriodNumber, token.MaxConsecutive))
                    continue;
                if (!CheckConsecutiveLimit(teacherDaySchedules, token.TeacherId, day, second.PeriodNumber, token.MaxConsecutive))
                    continue;

                foreach (var room in eligibleRooms)
                {
                    var score = ScoreSlot(token, day, first, periods, teacherDaySchedules, teacherSlots);
                    candidates.Add((day, first.Id, second.Id, room.Id, score));
                }
            }
        }

        if (candidates.Count == 0)
            return null;

        var best = candidates.OrderByDescending(c => c.Score).First();
        return (best.Day, best.FirstPeriodId, best.SecondPeriodId, best.RoomId);
    }

    private static bool CheckConsecutiveLimit(
        Dictionary<int, List<(int Day, int PeriodNumber)>> teacherDaySchedules,
        int teacherId, int day, int periodNum, int maxConsecutive)
    {
        if (maxConsecutive <= 0)
            return true;

        var scheduledPeriods = teacherDaySchedules.TryGetValue(teacherId, out var sched)
            ? sched.Where(s => s.Day == day).Select(s => s.PeriodNumber).ToHashSet()
            : [];

        int runForward = 0;
        for (int p = periodNum; p >= 1; p--)
        {
            if (scheduledPeriods.Contains(p) || p == periodNum)
            {
                runForward++;
                if (runForward > maxConsecutive)
                    return false;
            }
            else
            {
                break;
            }
        }

        int runBackward = 0;
        for (int p = periodNum; p <= 20; p++)
        {
            if (scheduledPeriods.Contains(p) || p == periodNum)
            {
                runBackward++;
                if (runBackward > maxConsecutive)
                    return false;
            }
            else
            {
                break;
            }
        }

        return true;
    }

    private static int ScoreSlot(
        PlacementToken token,
        int day,
        RoutinePeriod period,
        IReadOnlyList<RoutinePeriod> allPeriods,
        Dictionary<int, List<(int Day, int PeriodNumber)>> teacherDaySchedules,
        Dictionary<(int Day, int PeriodId), int> teacherSlots)
    {
        int score = 0;
        int totalPeriods = allPeriods.Max(p => p.PeriodNumber);

        int morningThreshold = Math.Max(1, totalPeriods / 3);

        if (token.Priority >= 8 && period.PeriodNumber <= morningThreshold)
            score += 100;
        else if (token.Priority >= 5 && period.PeriodNumber <= morningThreshold)
            score += 50;
        else if (token.Priority <= 3 && period.PeriodNumber > totalPeriods - 2)
            score += 30;

        if (token.Priority >= 9 && period.PeriodNumber == totalPeriods)
            score -= 50;

        if (teacherDaySchedules.TryGetValue(token.TeacherId, out var teacherSched))
        {
            int periodsOnDay = teacherSched.Count(s => s.Day == day);
            score += Math.Max(0, 60 - (periodsOnDay * 10));

            var dayPeriods = teacherSched.Where(s => s.Day == day).Select(s => s.PeriodNumber).ToHashSet();
            if (dayPeriods.Contains(period.PeriodNumber - 1) || dayPeriods.Contains(period.PeriodNumber + 1))
                score += 40;
        }

        return score;
    }

    private static async Task ResolveConflictsAsync(
        RoutineGenerationResult result,
        List<PlacementToken> tokens,
        List<RoutineEntry> placedEntries,
        Dictionary<(int Day, int PeriodId), int> sessionTeacherSlots,
        Dictionary<(int Day, int PeriodId), int> sessionRoomSlots,
        Dictionary<(int ClassId, int? SectionId, int? GroupId, int Day, int PeriodId), bool> sessionStudentSlots,
        Dictionary<int, List<(int Day, int PeriodNumber)>> sessionTeacherDaySchedules,
        HashSet<int> workingDayNumbers,
        IReadOnlyList<RoutinePeriod> periods,
        Dictionary<int, int> periodNumbers,
        Dictionary<int, RoutinePeriod> periodLookup,
        IReadOnlyList<RoutineEnt.Room> rooms,
        HashSet<(int TeacherId, int DayNumber, int RoutinePeriodId)> availabilityLookup,
        HashSet<(int TeacherId, int SubjectId, int ClassId)> unresolvableTeacherSubjects,
        int academicYearId,
        string createdBy)
    {
        foreach (var conflict in result.Conflicts.ToList())
        {
            if (conflict.TeacherId == null || conflict.SubjectId == null || conflict.ClassId == null)
                continue;

            var key = (conflict.TeacherId.Value, conflict.SubjectId.Value, conflict.ClassId.Value);
            if (unresolvableTeacherSubjects.Contains(key))
                continue;

            var conflictToken = tokens.FirstOrDefault(t =>
                t.TeacherId == conflict.TeacherId &&
                t.SubjectId == conflict.SubjectId &&
                t.ClassId == conflict.ClassId);

            if (conflictToken == null)
                continue;

            var conflictedEntries = placedEntries
                .Where(e => e.TeacherId == conflictToken.TeacherId &&
                            e.SubjectId == conflictToken.SubjectId &&
                            e.ClassId == conflictToken.ClassId)
                .ToList();

            foreach (var ce in conflictedEntries)
            {
                var ck = (ce.DayNumber, ce.RoutinePeriodId);
                sessionTeacherSlots.Remove(ck);
                sessionRoomSlots.Remove(ck);
                sessionStudentSlots.Remove((ce.ClassId, ce.SectionId, ce.GroupId, ce.DayNumber, ce.RoutinePeriodId));
            }

            var altSlot = FindBestSlot(
                conflictToken, workingDayNumbers, periods, periodNumbers, periodLookup, rooms,
                sessionTeacherSlots, sessionRoomSlots, sessionStudentSlots,
                sessionTeacherDaySchedules, availabilityLookup);

            if (altSlot.HasValue)
            {
                var (day, periodId, roomId) = altSlot.Value;

                foreach (var ce in conflictedEntries)
                    placedEntries.Remove(ce);

                placedEntries.Add(new RoutineEntry
                {
                    AcademicYearId = academicYearId,
                    ClassId = conflictToken.ClassId,
                    SectionId = conflictToken.SectionId,
                    GroupId = conflictToken.GroupId,
                    SubjectId = conflictToken.SubjectId,
                    TeacherId = conflictToken.TeacherId,
                    RoomId = roomId,
                    RoutinePeriodId = periodId,
                    DayNumber = day,
                    IsLab = conflictToken.RequiresLab,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                });

                var key2 = (day, periodId);
                sessionTeacherSlots[key2] = conflictToken.TeacherId;
                sessionRoomSlots[key2] = roomId;
                sessionStudentSlots[(conflictToken.ClassId, conflictToken.SectionId, conflictToken.GroupId, day, periodId)] = true;
                if (periodLookup.TryGetValue(periodId, out var altPeriod))
                {
                    if (!sessionTeacherDaySchedules.ContainsKey(conflictToken.TeacherId))
                        sessionTeacherDaySchedules[conflictToken.TeacherId] = [];
                    sessionTeacherDaySchedules[conflictToken.TeacherId].Add((day, altPeriod.PeriodNumber));
                }

                result.Conflicts.Remove(conflict);
                result.ConflictTokens--;
            }
            else
            {
                foreach (var ce in conflictedEntries)
                {
                    placedEntries.Add(ce);
                    var ck = (ce.DayNumber, ce.RoutinePeriodId);
                    sessionTeacherSlots[ck] = ce.TeacherId;
                    sessionRoomSlots[ck] = ce.RoomId;
                    sessionStudentSlots[(ce.ClassId, ce.SectionId, ce.GroupId, ce.DayNumber, ce.RoutinePeriodId)] = true;
                }
            }
        }
    }

    private record PlacementToken(
        int SubjectRequirementId,
        int ClassId,
        int? SectionId,
        int? GroupId,
        int SubjectId,
        int TeacherId,
        bool RequiresLab,
        bool RequiresDoublePeriod,
        int Priority,
        int MaxConsecutive
    );
}

public class RoutineGenerationResult
{
    public int GenerationId { get; set; }
    public int TotalTokens { get; set; }
    public int PlacedTokens { get; set; }
    public int ConflictTokens { get; set; }
    public bool Success => ConflictTokens == 0;
    public List<RoutineConflictInfo> Conflicts { get; set; } = [];
}

public class RoutineConflictInfo
{
    public string ConflictType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? TeacherId { get; set; }
    public int? RoomId { get; set; }
    public int? SubjectId { get; set; }
    public int? ClassId { get; set; }
    public int? RoutinePeriodId { get; set; }
    public int? DayNumber { get; set; }
}
