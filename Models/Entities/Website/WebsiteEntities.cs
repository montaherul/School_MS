using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public class SchoolSetting : BaseEntity
{
    [MaxLength(160)]
    public string SchoolName { get; set; } = string.Empty;

    [MaxLength(80)]
    public string ShortName { get; set; } = string.Empty;

    [MaxLength(160)]
    public string? BanglaName { get; set; }

    [MaxLength(20)]
    public string EIIN { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? SchoolCode { get; set; }

    public int? EstablishedYear { get; set; }

    [MaxLength(500)]
    public string? SchoolMotto { get; set; }

    [MaxLength(2000)]
    public string? SchoolDescription { get; set; }

    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Mobile { get; set; }

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(260)]
    public string Website { get; set; } = string.Empty;

    // Social Media
    [MaxLength(260)]
    public string? FacebookUrl { get; set; }

    [MaxLength(260)]
    public string? YouTubeUrl { get; set; }

    [MaxLength(260)]
    public string? InstagramUrl { get; set; }

    [MaxLength(260)]
    public string? LinkedInUrl { get; set; }

    [MaxLength(260)]
    public string? TwitterUrl { get; set; }

    // Branding
    [MaxLength(260)]
    public string? LogoPath { get; set; }

    [MaxLength(260)]
    public string? FaviconPath { get; set; }

    [MaxLength(260)]
    public string? LoginLogoPath { get; set; }

    [MaxLength(260)]
    public string? FooterLogoPath { get; set; }

    [MaxLength(260)]
    public string? WebsiteBannerPath { get; set; }

    // Principal Info
    [MaxLength(160)]
    public string? PrincipalName { get; set; }

    [MaxLength(160)]
    public string? PrincipalDesignation { get; set; }

    [MaxLength(4000)]
    public string? PrincipalMessage { get; set; }

    [MaxLength(260)]
    public string? PrincipalImagePath { get; set; }

    [MaxLength(260)]
    public string? PrincipalSignaturePath { get; set; }

    [MaxLength(500)]
    public string? PrincipalQualification { get; set; }

    // Content
    [MaxLength(2000)]
    public string? Mission { get; set; }

    [MaxLength(2000)]
    public string? Vision { get; set; }

    // Footer
    [MaxLength(500)]
    public string? FooterText { get; set; }

    [MaxLength(500)]
    public string? CopyrightText { get; set; }

    // Map
    [MaxLength(1000)]
    public string? GoogleMapEmbed { get; set; }

    // SEO
    [MaxLength(160)]
    public string? MetaTitle { get; set; }

    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    [MaxLength(500)]
    public string? MetaKeywords { get; set; }

    [MaxLength(260)]
    public string? OgImagePath { get; set; }

    [MaxLength(160)]
    public string? OgTitle { get; set; }

    [MaxLength(500)]
    public string? OgDescription { get; set; }

    // Welcome / Homepage Content
    [MaxLength(200)]
    public string? WelcomeHeading { get; set; }

    [MaxLength(500)]
    public string? WelcomeTagline { get; set; }

    [MaxLength(4000)]
    public string? WelcomeText { get; set; }

    [MaxLength(4000)]
    public string? SchoolHistory { get; set; }

    [MaxLength(200)]
    public string? OfficeHours { get; set; }

    // Statistics Labels
    [MaxLength(100)]
    public string? StudentLabel { get; set; }

    [MaxLength(100)]
    public string? TeacherLabel { get; set; }

    [MaxLength(100)]
    public string? EmployeeLabel { get; set; }

    [MaxLength(100)]
    public string? ClassLabel { get; set; }

    // Homepage Section Visibility Toggles
    public bool ShowSlider { get; set; } = true;
    public bool ShowPrincipalMessage { get; set; } = true;
    public bool ShowNotices { get; set; } = true;
    public bool ShowEvents { get; set; } = true;
    public bool ShowGallery { get; set; } = true;
    public bool ShowAdmissionCTA { get; set; } = true;
    public bool ShowStatistics { get; set; } = true;
    public bool ShowWelcomeSection { get; set; } = true;

    // Admission Page Settings
    public bool AdmissionEnabled { get; set; } = true;
    public bool OnlineAdmissionEnabled { get; set; } = true;
    public bool ShowAdmissionPage { get; set; } = true;
    public bool ShowAdmissionFees { get; set; } = true;
    public bool ShowAdmissionGuidelines { get; set; } = true;
    public bool ShowAdmissionRequirements { get; set; } = true;
    public bool ShowAdmissionDownloads { get; set; } = true;

    // Result Access
    public bool AllowResultWithDue { get; set; } = true;

    // Guardian Portal Feature Toggles
    public bool EnableGuardianPortal { get; set; } = false;
    public bool EnableGuardianActivation { get; set; } = false;
    public bool RequireGuardianForAdmission { get; set; } = false;
    public bool EnableGuardianNotifications { get; set; } = false;

    // Event Notification Settings
    public bool EnableEventEmailNotifications { get; set; } = false;
    public bool EnableStudentNotifications { get; set; } = false;
    public bool SendImmediately { get; set; } = true;
    public bool SendOnPublish { get; set; } = true;
    public bool DailyDigestMode { get; set; } = false;
    public int MaximumEmailsPerBatch { get; set; } = 50;
    public int? DefaultEventTemplateId { get; set; }

    [MaxLength(160)]
    public string? NotificationSenderName { get; set; }

    [MaxLength(160)]
    public string? NotificationSenderEmail { get; set; }

    // SMTP Server Settings
    [MaxLength(160)]
    public string? SmtpHost { get; set; }

    public int SmtpPort { get; set; } = 587;

    public bool SmtpEnableSsl { get; set; } = true;

    [MaxLength(160)]
    public string? SmtpUserName { get; set; }

    [MaxLength(260)]
    public string? SmtpPassword { get; set; }

    [MaxLength(160)]
    public string? SmtpFromEmail { get; set; }

    [MaxLength(260)]
    public string? BaseUrl { get; set; }

    [MaxLength(260)]
    public string? LocalUrl { get; set; }

    [MaxLength(260)]
    public string? PublicUrl { get; set; }

    // Event Approval Workflow Settings
    public bool EnableEventApprovalWorkflow { get; set; } = false;

    // Event Reminder Settings
    public bool EnableEventReminders { get; set; } = false;
    public int DefaultReminderTiming { get; set; } = 1;
    public ReminderUnit DefaultReminderUnit { get; set; } = ReminderUnit.Days;
    public int MaxRemindersPerEvent { get; set; } = 3;

    [MaxLength(200)]
    public string? AdmissionTitle { get; set; }

    [MaxLength(500)]
    public string? AdmissionSubtitle { get; set; }

    [MaxLength(4000)]
    public string? AdmissionGuidelines { get; set; }

    [MaxLength(4000)]
    public string? AdmissionEligibility { get; set; }

    [MaxLength(4000)]
    public string? AdmissionRequirements { get; set; }

    [MaxLength(4000)]
    public string? AdmissionProcess { get; set; }

    [MaxLength(2000)]
    public string? AdmissionFeeNote { get; set; }

    [MaxLength(200)]
    public string? AdmissionCtaTitle { get; set; }

    [MaxLength(500)]
    public string? AdmissionCtaText { get; set; }

    public DateTime? AdmissionOpenDate { get; set; }

    public DateTime? AdmissionCloseDate { get; set; }

    [MaxLength(260)]
    public string? AdmissionCircularPath { get; set; }

    [MaxLength(260)]
    public string? AdmissionFormPath { get; set; }

    // Admission SEO
    [MaxLength(160)]
    public string? AdmissionMetaTitle { get; set; }

    [MaxLength(500)]
    public string? AdmissionMetaDescription { get; set; }

    [MaxLength(500)]
    public string? AdmissionMetaKeywords { get; set; }

    [MaxLength(160)]
    public string? AdmissionOgTitle { get; set; }

    [MaxLength(500)]
    public string? AdmissionOgDescription { get; set; }

    [MaxLength(260)]
    public string? AdmissionOgImagePath { get; set; }
}

public class AdmissionFeeStructure : BaseEntity
{
    public int SchoolClassId { get; set; }

    [MaxLength(100)]
    public string ClassName { get; set; } = string.Empty;

    public decimal AdmissionFee { get; set; }

    public decimal MonthlyFee { get; set; }

    public decimal SessionFee { get; set; }

    public decimal ExamFee { get; set; }

    public decimal OtherFee { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}

public class WebsitePage : BaseEntity
{
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Slug { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    [MaxLength(160)]
    public string? MetaTitle { get; set; }

    [MaxLength(260)]
    public string? MetaDescription { get; set; }

    public bool IsPublished { get; set; } = true;
}

public class Slider : BaseEntity
{
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(260)]
    public string? Subtitle { get; set; }

    [MaxLength(50)]
    public string? ButtonText { get; set; }

    [MaxLength(260)]
    public string? ButtonUrl { get; set; }

    [MaxLength(260)]
    public string ImagePath { get; set; } = string.Empty;

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

public class Event : BaseEntity
{
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    public DateTime EventDate { get; set; } = DateTime.UtcNow;

    [MaxLength(160)]
    public string? EventLocation { get; set; }

    [MaxLength(260)]
    public string? CoverImagePath { get; set; }

    public bool IsUpcoming { get; set; } = true;
    public bool IsPublished { get; set; } = true;

    public EventApprovalStatus ApprovalStatus { get; set; } = EventApprovalStatus.Draft;

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    [MaxLength(60)]
    public string Category { get; set; } = "EventPublished";

    public ICollection<ReminderConfig> ReminderConfigs { get; set; } = new List<ReminderConfig>();
}

public class Gallery : BaseEntity
{
    [MaxLength(160)]
    public string AlbumName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(260)]
    public string? CoverImagePath { get; set; }

    public virtual ICollection<GalleryImage> Images { get; set; } = new List<GalleryImage>();
}

public class GalleryImage : BaseEntity
{
    public int GalleryId { get; set; }
    public virtual Gallery? Gallery { get; set; }

    [MaxLength(260)]
    public string ImagePath { get; set; } = string.Empty;

    [MaxLength(260)]
    public string? Caption { get; set; }

    public int DisplayOrder { get; set; } = 0;
}

public class Announcement : BaseEntity
{
    [MaxLength(260)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

public class ContactMessage : BaseEntity
{
    [MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Phone { get; set; }

    [MaxLength(260)]
    public string Subject { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Status { get; set; } = "Unread";
}

public class EmailTemplate : BaseEntity
{
    [MaxLength(160)]
    public string TemplateName { get; set; } = string.Empty;

    [MaxLength(260)]
    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Placeholders { get; set; }

    public bool IsActive { get; set; } = true;
}
