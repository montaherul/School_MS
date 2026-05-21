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
                PrincipalMessage = "Welcome to CCSC. For over a century, our institution has stood as a beacon of standard education, character cultivation, and national values in Bangladesh. We welcome all parents and students to join our progressive academic family.",
                PrincipalImagePath = "https://images.unsplash.com/photo-1544717305-2782549b5136?auto=format&fit=crop&q=80&w=400",
                LogoPath = "/images/default-logo.png",
                FaviconPath = "/favicon.ico",
                FacebookUrl = "https://facebook.com/collegiate.school",
                YouTubeUrl = "https://youtube.com/collegiate.school",
                Mission = "To provide balanced, modern, and value-based education that equips students with critical thinking skills, high moral standards, and patriotic feelings.",
                Vision = "To remain a premier academic center of secondary and higher education in Bangladesh, forming leaders of tomorrow through innovation and comprehensive extracurricular excellence.",
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

        await _db.SaveChangesAsync();
    }
}
