# Public Website Module - System Implementation Report

We have successfully designed, built, integrated, and verified the complete **Public School Website Module** for the Bangladesh School Management System! The module is fully functional, beautifully styled, SEO-optimized, and compiles with **zero build errors**.

---

## 📸 Architectural Visual Showcase
Here is the official render of the newly deployed public portal home page:

![Central Public Portal](/C:/Users/islam/.gemini/antigravity/brain/d4b7d4e6-3e8f-49f6-9cee-caed93a443e9/landing_page_full_top_1779037055838.png)

---

## 🛠️ System Architecture & Layered Implementation

The public website was engineered inside the core MVC project using a strict **Repository-Service-UnitOfWork layered enterprise pattern**:

### 1. Database Schema & Migration (`Models/Entities/Website`)
We created Entity Framework models with full support for auditing (`ISoftDelete`, `IAuditable`):
- **`SchoolSetting`**: Holds the global institution profile (Name, EIIN, Address, Contacts, Principal Bio, Logo, Map, and Footer details).
- **`Slider`**: Manages the high-resolution homepage slideshow.
- **`WebsitePage`**: Lightweight dynamic CMS engine for custom pages using slugs.
- **`Event`**: Academic and cultural events calendar with geolocation and upcoming counters.
- **`Gallery` & `GalleryImage`**: Campus photo album system.

> [!NOTE]
> All database migrations were generated and applied successfully using Entity Framework Core. All portal tables exist in the SQL Server database.

### 2. Business Services Layer (`Services/Interfaces/Website` & `Services/Implementations/Website`)
- **`SchoolWebsiteService`**: Business operations to manage global setting records.
- **`SliderService`**, **`NoticeService`**, **`EventService`**, **`GalleryService`**, **`WebsitePageService`**: Modular services for retrieving active records for public views and saving back-office additions.
- **`WebsiteSeeder`**: Automatic database initializer that seeds CCSC Chattogram Collegiate template configurations during startup!

### 3. Controller Actions (`Controllers/Common`)
- **`HomeController`**: Exposes `/` (Home), `/about`, `/principal-message`, `/mission-vision`, `/contact`, `/admission`.
- **`NoticeController`**: Exposes `/notices` (search-enabled bulletins list) and `/notice/details/{id}`.
- **`EventController`**: Exposes `/events` (events calendar) and `/event/details/{id}`.
- **`GalleryController`**: Exposes `/gallery` (album lists) and `/gallery/album/{id}` (lightbox image galleries).
- **`PageController`**: Exposes dynamic CMS engine `/p/{slug}` routing address.
- **`WebsiteAdminController`**: Exposes **`/Admin/Website`** back-office console with AJAX grids.

---

## 💎 Rich Premium Design Aesthetics

The interface adopts a high-end educational branding style:
- **`--edu-navy` (`#0B2545`)** & **`--edu-dark` (`#081C33`)** for elite authority.
- **`--edu-gold` (`#EEB902`)** for accents, call-to-actions, and highlights.
- **`--edu-blue` (`#134074`)** for secondary controls.
- Fully responsive Bootstrap 5 layout with smooth CSS card transitions, dark headers, responsive navigation toggles, and detailed copyright footer columns.
- Integrated **Quill WYSIWYG Editor** on CMS editor pages.
- Pure JS **Lightbox viewer** inside photo galleries.
- Real-time countdown clocks for upcoming academic events.

---

## 🖥️ Portal Routing Navigation Table

| Page / Route | Component View | Description |
| :--- | :--- | :--- |
| **`/`** | `Views/Home/Index` | Dynamic homepage with sliders, alert ticks, notices, and counters. |
| **`/about`** | `Views/Home/About` | Institution at a glance, history, levels, and levels breakdown. |
| **`/principal-message`** | `Views/Home/PrincipalMessage` | Principal address bio statement with handwritten gold signature. |
| **`/mission-vision`** | `Views/Home/MissionVision` | Strategic institutional statements and value guidelines. |
| **`/admission`** | `Views/Home/Admission` | Class guidelines, session fees structure table, and offline circulars. |
| **`/contact`** | `Views/Home/Contact` | Maps embed block, phone/email contact grid, and helpline feedback form. |
| **`/notices`** | `Views/Notice/Index` | Notice board lists with search filter fields. |
| **`/events`** | `Views/Event/Index` | School calendars featuring dynamic countdown clocks. |
| **`/gallery`** | `Views/Gallery/Index` | Campus photos grouped by album directory. |
| **`/Admin/Website`** | `Views/WebsiteAdmin/Index` | **Tabulator Admin Console** with separate tabs for quick CRUD management. |
