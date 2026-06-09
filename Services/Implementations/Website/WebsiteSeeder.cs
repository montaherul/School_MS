using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Entities.Communication;

namespace SchoolManagementSystem.Services.Implementations.Website;

public class WebsiteSeeder
{
    private readonly SchoolDbContext _db;

    public WebsiteSeeder(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync()
    {
        // 1. Seed School Settings
        if (!await _db.SchoolSettings.AnyAsync(s => !s.IsDeleted))
        {
            var settings = new SchoolSetting
            {
                SchoolName = "Chattogram Collegiate School & College",
                ShortName = "CCSC",
                EIIN = "104298",
                Email = "info@collegiate-school.edu.bd",
                Phone = "+880 31 610429",
                Address = "Ice Factory Road, Double Mooring, Chattogram, Bangladesh",
                PrincipalName = "Prof. Muhammad Ashraful Islam",
                PrincipalDesignation = "Principal",
                PrincipalMessage = "Welcome to CCSC. For over a century, our institution has stood as a beacon of standard education, character cultivation, and national values in Bangladesh. We welcome all parents and students to join our progressive academic family.",
                PrincipalImagePath = "https://images.unsplash.com/photo-1544717305-2782549b5136?auto=format&fit=crop&q=80&w=400",
                LogoPath = "/images/default-logo.png",
                FaviconPath = "/favicon.ico",
                LoginLogoPath = "/images/default-login-logo.png",
                FooterLogoPath = "/images/default-footer-logo.png",
                FacebookUrl = "https://facebook.com/collegiate.school",
                YouTubeUrl = "https://youtube.com/collegiate.school",
                Mission = "To provide balanced, modern, and value-based education that equips students with critical thinking skills, high moral standards, and patriotic feelings.",
                Vision = "To remain a premier academic center of secondary and higher education in Bangladesh, forming leaders of tomorrow through innovation and comprehensive extracurricular excellence.",
                SchoolMotto = "Knowledge, Discipline, Excellence",
                WelcomeHeading = "Welcome to Our Academic Portal",
                WelcomeTagline = "Gateway to Knowledge & Excellence",
                WelcomeText = "We are delighted to welcome you to the official academic portal of Chattogram Collegiate School & College. Our institution, established in 1836, stands as a testament to Bangladesh's commitment to quality education and character development. Through our comprehensive curriculum and dedicated faculty, we strive to nurture critical thinkers, responsible citizens, and future leaders who will contribute meaningfully to society.",
                SchoolHistory = "Founded in 1836 as Chittagong Government High School, Chattogram Collegiate School & College has been a beacon of education in Bangladesh for nearly two centuries. The institution has evolved through various phases of Bangladesh's history, maintaining its commitment to academic excellence and character building. Today, it stands as one of the most prestigious educational institutions in the country, producing leaders in various fields including politics, academia, business, and public service.",
                OfficeHours = "Sat - Thu (8:00 AM - 2:00 PM)",
                StudentLabel = "Active Students",
                TeacherLabel = "Honorable Teachers",
                EmployeeLabel = "Staff Members",
                ClassLabel = "Classrooms",
                GoogleMapEmbed = "<iframe src=\"https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3690.3129598285514!2d91.82390297592472!3d22.341819341499596!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x30acd89686036329%3A0xe100c5c56c2d1b09!2sChattogram%20Collegiate%20School!5e0!3m2!1sen!2sbd!4v1700000000000!5m2!1sen!2sbd\" width=\"100%\" height=\"350\" style=\"border:0;\" allowfullscreen=\"\" loading=\"lazy\"></iframe>",
                FooterText = "© 2026 Chattogram Collegiate School & College. All rights reserved. Managed by Ministry of Education, Bangladesh.",
                CreatedBy = "seeder",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            await _db.SchoolSettings.AddAsync(settings);
        }

        // 2. Seed Hero Sliders
        if (!await _db.Sliders.AnyAsync(s => !s.IsDeleted))
        {
            var sliders = new[]
            {
                new Slider
                {
                    Title = "Shaping the Future Leaders of Bangladesh",
                    Subtitle = "Over 185 years of academic standard, discipline, and success.",
                    ImagePath = "https://images.unsplash.com/photo-1523050854058-8df90110c9f1?auto=format&fit=crop&q=80&w=1920",
                    ButtonText = "Apply For Admission",
                    ButtonUrl = "/Admission/Apply",
                    DisplayOrder = 1,
                    IsActive = true,
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.UtcNow
                },
                new Slider
                {
                    Title = "Modern STEM Laboratories & Library",
                    Subtitle = "Fostering academic inquiry through dynamic hands-on scientific projects.",
                    ImagePath = "https://images.unsplash.com/photo-1562774053-701939374585?auto=format&fit=crop&q=80&w=1920",
                    ButtonText = "Explore Facilities",
                    ButtonUrl = "/p/facilities",
                    DisplayOrder = 2,
                    IsActive = true,
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.UtcNow
                }
            };
            await _db.Sliders.AddRangeAsync(sliders);
        }

        // 3. Seed Bulletins / Notices
        if (!await _db.Notices.AnyAsync(n => !n.IsDeleted))
        {
            var notices = new[]
            {
                new Notice
                {
                    Title = "Admission Open for Session 2026",
                    Body = "Online registration portal is open for admission applications for Classes VI - IX. The deadline to register is 15th December 2026. Make sure to download the prospectus for details.",
                    AudienceRole = "Public",
                    PublishAt = DateTime.UtcNow.AddDays(-2),
                    IsPublished = true,
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.UtcNow
                },
                new Notice
                {
                    Title = "Annual SSC Test Examination Routine",
                    Body = "The upcoming test exam routine for SSC candidates has been published. Examinations start from 10th November. Routine copies can be downloaded from the attachment below.",
                    AudienceRole = "Public",
                    PublishAt = DateTime.UtcNow.AddDays(-1),
                    IsPublished = true,
                    AttachmentPath = "/docs/ssc_test_routine_2026.pdf",
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.UtcNow
                }
            };
            await _db.Notices.AddRangeAsync(notices);
        }

        // 4. Seed Events
        if (!await _db.Events.AnyAsync(e => !e.IsDeleted))
        {
            var events = new[]
            {
                new Event
                {
                    Title = "Annual Sports Day & Athletes Championship",
                    Description = "Our signature field athletics competition featuring inter-house tournaments, sprint runs, high jump championships, and a grand prize distribution event headed by the Deputy Commissioner of Chattogram.",
                    EventDate = DateTime.UtcNow.AddDays(15),
                    EventLocation = "Main School Playground",
                    CoverImagePath = "https://images.unsplash.com/photo-1517649763962-0c623066013b?auto=format&fit=crop&q=80&w=800",
                    IsUpcoming = true,
                    IsPublished = true,
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.UtcNow
                },
                new Event
                {
                    Title = "National Science & Robotics Fest 2026",
                    Description = "CCSC hosts the premier secondary school science Olympiad. Dynamic model creations, biology exhibitions, logic debate sessions, and a live drone racing showcase from top institutions in the division.",
                    EventDate = DateTime.UtcNow.AddDays(30),
                    EventLocation = "Auditorium & Physics Wing",
                    CoverImagePath = "https://images.unsplash.com/photo-1485827404703-89b55fcc595e?auto=format&fit=crop&q=80&w=800",
                    IsUpcoming = true,
                    IsPublished = true,
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.UtcNow
                }
            };
            await _db.Events.AddRangeAsync(events);
        }

        // 5. Seed CMS Pages
        if (!await _db.WebsitePages.AnyAsync(p => !p.IsDeleted))
        {
            var page = new WebsitePage
            {
                Title = "Our Facilities & Campus",
                Slug = "facilities",
                MetaTitle = "Facilities - Chattogram Collegiate School",
                MetaDescription = "Explore the infrastructure, laboratories, and dynamic library facilities of Chattogram Collegiate.",
                Content = @"
                    <h3>Modern Infrastructure & Amenities</h3>
                    <p>At chattogram collegiate, we believe that world-class facilities catalyze higher academic retention. Our modern campus is designed to foster a safe, positive, and rich learning environment.</p>
                    
                    <h4>1. Science & Tech Wing</h4>
                    <p>Equipped with specialized physics, chemistry, and biology experimental labs. Includes modern equipment such as digital compound microscopes, molecular modeling kits, and premium robotics boards.</p>
                    
                    <h4>2. Centenary Library</h4>
                    <p>Fostering a love for reading with over 8,000 reference books, national encyclopedias, digital research terminals with broadband internet, and quiet study alcoves.</p>
                    
                    <h4>3. Smart Classroom Systems</h4>
                    <p>Multimedia classrooms equipped with high-brightness LED projectors and digital touch interactive displays, allowing teachers to deliver visually rich animated courses.</p>
                ",
                IsPublished = true,
                CreatedBy = "seeder",
                CreatedAt = DateTime.UtcNow
            };
            await _db.WebsitePages.AddAsync(page);
        }

        // 6. Seed Email Templates
        if (!await _db.EmailTemplates.AnyAsync(t => !t.IsDeleted))
        {
            var templates = new[]
            {
                new EmailTemplate
                {
                    TemplateName = "EmployeeInvitation",
                    Subject = "You're invited to join {SchoolName}",
                    Body = @"<h2>Welcome to {SchoolName}!</h2>
<p>Dear {EmployeeName},</p>
<p>You have been invited to join {SchoolName} as a <strong>{Designation}</strong>.</p>
<p>Please use the following credentials to log in and complete your profile:</p>
<ul>
<li><strong>Portal URL:</strong> <a href=""{PortalUrl}"">{PortalUrl}</a></li>
<li><strong>Email:</strong> {Email}</li>
<li><strong>Temporary Password:</strong> {Password}</li>
</ul>
<p>For security reasons, please change your password after your first login.</p>
<p>Best regards,<br/>{PrincipalName}<br/>Principal, {SchoolName}</p>",
                    Placeholders = "{SchoolName},{EmployeeName},{Designation},{PortalUrl},{Email},{Password},{PrincipalName}",
                    IsActive = true,
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.UtcNow
                },
                new EmailTemplate
                {
                    TemplateName = "PasswordReset",
                    Subject = "Password Reset Request - {SchoolName}",
                    Body = @"<h2>Password Reset</h2>
<p>Dear {UserName},</p>
<p>We received a request to reset your password for your {SchoolName} account.</p>
<p>Your new temporary password is: <strong>{Password}</strong></p>
<p>Please log in at <a href=""{PortalUrl}"">{PortalUrl}</a> and change your password immediately.</p>
<p>If you did not request this, please contact the administration.</p>
<p>Best regards,<br/>{SchoolName} Administration</p>",
                    Placeholders = "{SchoolName},{UserName},{Password},{PortalUrl}",
                    IsActive = true,
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.UtcNow
                },
                new EmailTemplate
                {
                    TemplateName = "AttendanceAbsent",
                    Subject = "Attendance Alert - {EmployeeName} was absent on {Date}",
                    Body = @"<h2>Attendance Notification</h2>
<p>This is an automated notification regarding attendance.</p>
<p><strong>Employee:</strong> {EmployeeName}<br/>
<strong>Designation:</strong> {Designation}<br/>
<strong>Date:</strong> {Date}<br/>
<strong>Status:</strong> Absent</p>
<p>Please ensure proper documentation is submitted as per school policy.</p>
<p>Best regards,<br/>Attendance Management System<br/>{SchoolName}</p>",
                    Placeholders = "{EmployeeName},{Designation},{Date},{SchoolName}",
                    IsActive = true,
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.UtcNow
                },
                new EmailTemplate
                {
                    TemplateName = "WelcomeEmail",
                    Subject = "Welcome to {SchoolName}!",
                    Body = @"<h2>Welcome Aboard!</h2>
<p>Dear {UserName},</p>
<p>Welcome to <strong>{SchoolName}</strong>! We are delighted to have you as part of our community.</p>
<p>Your account has been created successfully. Here are your details:</p>
<ul>
<li><strong>Portal URL:</strong> <a href=""{PortalUrl}"">{PortalUrl}</a></li>
<li><strong>Email:</strong> {Email}</li>
<li><strong>Temporary Password:</strong> {Password}</li>
</ul>
<p>Please log in and update your profile at your earliest convenience.</p>
<p>Best regards,<br/>{PrincipalName}<br/>Principal, {SchoolName}</p>",
                    Placeholders = "{SchoolName},{UserName},{PortalUrl},{Email},{Password},{PrincipalName}",
                    IsActive = true,
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.UtcNow
                }
            };
            await _db.EmailTemplates.AddRangeAsync(templates);
        }

        // 7. Seed Announcements
        if (!await _db.Announcements.AnyAsync(a => !a.IsDeleted))
        {
            var announcements = new[]
            {
                new Announcement
                {
                    Title = "Admission Open for Session 2026",
                    Content = "Admissions are now open for Classes VI-XII for the 2026 academic session. Apply online through our admission portal. Deadline: 31 December 2026.",
                    IsActive = true,
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.UtcNow
                },
                new Announcement
                {
                    Title = "Annual Sports Day 2026",
                    Content = "Our Annual Sports Day will be held on 15th February 2026. All students are encouraged to participate. Registration opens 1st February.",
                    IsActive = true,
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.UtcNow
                },
                new Announcement
                {
                    Title = "SSC Examination Schedule Published",
                    Content = "The SSC Examination 2026 schedule has been published. Students can download the routine from the Notice Board section.",
                    IsActive = true,
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.UtcNow
                }
            };
            await _db.Announcements.AddRangeAsync(announcements);
        }

        // 8. Seed Contact Notification Email Template
        if (!await _db.EmailTemplates.AnyAsync(t => t.TemplateName == "ContactNotification" && !t.IsDeleted))
        {
            var contactTemplate = new EmailTemplate
            {
                TemplateName = "ContactNotification",
                Subject = "New Contact Form Submission - {SchoolName}",
                Body = @"<h2>New Contact Form Submission</h2>
<p><strong>School:</strong> {SchoolName}</p>
<p><strong>From:</strong> {Name} ({Email})</p>
<p><strong>Phone:</strong> {Phone}</p>
<p><strong>Subject:</strong> {Subject}</p>
<p><strong>Message:</strong></p>
<p>{Message}</p>
<hr/>
<p><small>Submitted on {Timestamp}</small></p>",
                Placeholders = "{SchoolName},{Name},{Email},{Phone},{Subject},{Message},{Timestamp}",
                IsActive = true,
                CreatedBy = "seeder",
                CreatedAt = DateTime.UtcNow
            };
            await _db.EmailTemplates.AddAsync(contactTemplate);
        }

        await _db.SaveChangesAsync();
    }
}
