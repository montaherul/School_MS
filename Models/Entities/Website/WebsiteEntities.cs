using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public class SchoolSetting : BaseEntity
{
    [MaxLength(100)]
    public string? SchoolName { get; set; }

    [MaxLength(100)]
    public string? ShortName { get; set; }

    [MaxLength(50)]
    public string? BanglaName { get; set; }

    [MaxLength(100)]
    public string? EIIN { get; set; }

    [MaxLength(100)]
    public string? SchoolCode { get; set; }

    public int? EstablishedYear { get; set; }

    [MaxLength(200)]
    public string? SchoolMotto { get; set; }

    [MaxLength(500)]
    public string? SchoolDescription { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(20)]
    public string? Mobile { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Website { get; set; }

    [MaxLength(200)]
    public string? FacebookUrl { get; set; }

    [MaxLength(200)]
    public string? YouTubeUrl { get; set; }

    [MaxLength(200)]
    public string? InstagramUrl { get; set; }

    [MaxLength(200)]
    public string? LinkedInUrl { get; set; }

    [MaxLength(200)]
    public string? TwitterUrl { get; set; }

    [MaxLength(200)]
    public string? LogoPath { get; set; }

    [MaxLength(200)]
    public string? FaviconPath { get; set; }

    [MaxLength(200)]
    public string? LoginLogoPath { get; set; }

    [MaxLength(200)]
    public string? FooterLogoPath { get; set; }

    [MaxLength(200)]
    public string? WebsiteBannerPath { get; set; }

    [MaxLength(100)]
    public string? PrincipalName { get; set; }

    [MaxLength(100)]
    public string? PrincipalDesignation { get; set; }

    [MaxLength(500)]
    public string? PrincipalMessage { get; set; }

    [MaxLength(200)]
    public string? PrincipalImagePath { get; set; }

    [MaxLength(200)]
    public string? PrincipalSignaturePath { get; set; }

    [MaxLength(500)]
    public string? PrincipalQualification { get; set; }

    [MaxLength(500)]
    public string? Mission { get; set; }

    [MaxLength(500)]
    public string? Vision { get; set; }

    [MaxLength(500)]
    public string? FooterText { get; set; }

    [MaxLength(500)]
    public string? CopyrightText { get; set; }

    [MaxLength(1000)]
    public string? GoogleMapEmbed { get; set; }

    [MaxLength(100)]
    public string? MetaTitle { get; set; }

    [MaxLength(200)]
    public string? MetaDescription { get; set; }

    [MaxLength(200)]
    public string? MetaKeywords { get; set; }

    [MaxLength(200)]
    public string? OgImagePath { get; set; }

    [MaxLength(100)]
    public string? OgTitle { get; set; }

    [MaxLength(200)]
    public string? OgDescription { get; set; }

    [MaxLength(100)]
    public string? WelcomeHeading { get; set; }

    [MaxLength(100)]
    public string? WelcomeTagline { get; set; }

    [MaxLength(500)]
    public string? WelcomeText { get; set; }

    [MaxLength(1000)]
    public string? SchoolHistory { get; set; }

    [MaxLength(200)]
    public string? OfficeHours { get; set; }

    [MaxLength(50)]
    public string? StudentLabel { get; set; }

    [MaxLength(50)]
    public string? TeacherLabel { get; set; }

    [MaxLength(50)]
    public string? EmployeeLabel { get; set; }

    [MaxLength(50)]
    public string? ClassLabel { get; set; }

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

    [MaxLength(100)]
    public string? AdmissionTitle { get; set; } = "Admission";

    [MaxLength(200)]
    public string? AdmissionSubtitle { get; set; }

    [MaxLength(1000)]
    public string? AdmissionGuidelines { get; set; }

    [MaxLength(500)]
    public string? AdmissionEligibility { get; set; }

    [MaxLength(500)]
    public string? AdmissionRequirements { get; set; }

    [MaxLength(1000)]
    public string? AdmissionProcess { get; set; }

    [MaxLength(500)]
    public string? AdmissionFeeNote { get; set; }

    [MaxLength(100)]
    public string? AdmissionCtaTitle { get; set; }

    [MaxLength(200)]
    public string? AdmissionCtaText { get; set; }

    public DateTime? AdmissionOpenDate { get; set; }

    public DateTime? AdmissionCloseDate { get; set; }

    [MaxLength(200)]
    public string? AdmissionCircularPath { get; set; }

    [MaxLength(200)]
    public string? AdmissionFormPath { get; set; }

    // Admission SEO
    [MaxLength(100)]
    public string? AdmissionMetaTitle { get; set; }

    [MaxLength(200)]
    public string? AdmissionMetaDescription { get; set; }

    [MaxLength(200)]
    public string? AdmissionMetaKeywords { get; set; }

    [MaxLength(100)]
    public string? AdmissionOgTitle { get; set; }

    [MaxLength(200)]
    public string? AdmissionOgDescription { get; set; }

    [MaxLength(200)]
    public string? AdmissionOgImagePath { get; set; }

    public bool AllowResultWithDue { get; set; } = false;

    // Portal Feature Toggles
    public bool EnableStudentPortal { get; set; } = false;

    public bool EnableGuardianPortal { get; set; } = false;

    public bool EnableGuardianActivation { get; set; } = false;

    public bool RequireGuardianForAdmission { get; set; } = false;

    public bool EnableGuardianNotifications { get; set; } = false;

    // Event Notification Settings
    public bool EnableEventEmailNotifications { get; set; } = true;

    public bool EnableStudentNotifications { get; set; } = true;

    public bool SendImmediately { get; set; } = false;

    public bool SendOnPublish { get; set; } = true;

    public bool DailyDigestMode { get; set; } = false;

    public int MaximumEmailsPerBatch { get; set; } = 100;

    public int? DefaultEventTemplateId { get; set; }

    [MaxLength(100)]
    public string? NotificationSenderName { get; set; }

    [MaxLength(100)]
    public string? NotificationSenderEmail { get; set; }

    [MaxLength(100)]
    public string? SmtpHost { get; set; }

    public int SmtpPort { get; set; } = 587;

    public bool SmtpEnableSsl { get; set; } = true;

    [MaxLength(100)]
    public string? SmtpUserName { get; set; }

    [MaxLength(200)]
    public string? SmtpPassword { get; set; }

    [MaxLength(100)]
    public string? SmtpFromEmail { get; set; }

    [MaxLength(200)]
    public string? BaseUrl { get; set; }

    [MaxLength(200)]
    public string? LocalUrl { get; set; }

    [MaxLength(200)]
    public string? PublicUrl { get; set; }

    public bool EnableEventApprovalWorkflow { get; set; }

    public bool EnableEventReminders { get; set; }

    public int DefaultReminderTiming { get; set; }

    public ReminderUnit DefaultReminderUnit { get; set; }

    public int MaxRemindersPerEvent { get; set; }

    public int GroupStartsFromClassId { get; set; }

    public bool AllowDirectAdmissionToClass10 { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(64)]
    public string? UpdatedBy { get; set; }
}