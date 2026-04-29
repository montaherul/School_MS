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
