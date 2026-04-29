using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Exam;

public class Exam : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int AcademicYearId { get; set; }
    public DateOnly StartsOn { get; set; }
    public DateOnly EndsOn { get; set; }
}

public class ExamSubject : BaseEntity
{
    public int ExamId { get; set; }
    public int SubjectId { get; set; }
    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
}

public class ExamSchedule : BaseEntity
{
    public int ExamId { get; set; }
    public int SubjectId { get; set; }
    public DateOnly ExamDate { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }

    [MaxLength(80)]
    public string RoomNo { get; set; } = string.Empty;
}

public class AdmitCard : BaseEntity
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }

    [MaxLength(40)]
    public string CardNo { get; set; } = string.Empty;
}

public class SeatingPlan : BaseEntity
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }

    [MaxLength(40)]
    public string SeatNo { get; set; } = string.Empty;
}
