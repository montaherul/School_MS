namespace SchoolManagementSystem.Models.Enums;

public enum AccountStatus { Active = 1, Inactive = 2, Locked = 3, Pending = 4 }
public enum AdmissionStatus { Pending = 1, Approved = 2, Rejected = 3, Converted = 4 }
public enum StudentStatus { Active = 1, Inactive = 2, Graduated = 3, Transferred = 4 }
public enum AttendanceStatus { Present = 1, Absent = 2, Late = 3, Leave = 4 }
public enum LeaveStatus { Pending = 1, Approved = 2, Rejected = 3 }
public enum PaymentMethod { Cash = 1, Bank = 2, Card = 3, MobileBanking = 4, Online = 5 }
public enum PaymentStatus { Unpaid = 1, Partial = 2, Paid = 3, Waived = 4 }
public enum PublishStatus { Draft = 1, PendingApproval = 2, Approved = 3, Published = 4 }
public enum NotificationChannel { InApp = 1, Email = 2, Sms = 3 }
public enum AssignmentStatus { Open = 1, Closed = 2, Graded = 3 }
public enum TeacherStatus { Active = 1, OnLeave = 2, Resigned = 3, Terminated = 4, Inactive = 5 }
public enum ExamTerm 
{ 
    FirstTerminal = 1,      // Class 1-5
    HalfYearly = 2,         // Class 6-10
    SecondTerminal = 3,     // Class 1-5
    Annual = 4,             // Class 1-10
    Final = 5,              // Class 1-5
    PreTest = 6,            // Class 10
    Test = 7,               // Class 10
    Other = 8
}
public enum AcademicEventType
{
    Holiday = 1,
    WeeklyOff = 2,
    Exam = 3,
    Vacation = 4,
    Event = 5
}
public enum ResultWorkflowStatus { Draft = 1, Submitted = 2, Reviewed = 3, Approved = 4, Published = 5, Locked = 6, Unpublished = 7 }

public enum OptionalSubjectMode { Disabled = 0, ExcludeFromGPA = 1, BonusGPA = 2, BestOf = 3, Custom = 4 }

public enum FailSubjectMode { StrictFail = 0, ExcludeFail = 1, Custom = 2 }
public enum ReEvaluationStatus { Requested = 1, Approved = 2, Rejected = 3, Revised = 4 }
public enum PromotionStatus { Pending = 1, Promoted = 2, Repeat = 3, Failed = 4 }
public enum AttendanceSessionStatus { Draft = 1, Submitted = 2, Locked = 3, Revised = 4, Approved = 5 }
