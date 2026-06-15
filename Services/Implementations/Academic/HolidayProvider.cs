using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class HolidayProvider
{
    public static List<HolidayMaster> GetAllAnnualHolidays(int year)
    {
        var holidays = new List<HolidayMaster>();
        int order = 0;

        void Add(string name, string? nameBn, DateOnly date, string type, string? religion = null, bool recurring = true)
        {
            order++;
            holidays.Add(new HolidayMaster
            {
                Name = name,
                NameBn = nameBn,
                HolidayDate = date,
                HolidayType = type,
                IsRecurring = recurring,
                Religion = religion,
                Description = null,
                DisplayOrder = order,
                IsActive = true,
                CountryCode = "BD"
            });
        }

        Add("International Mother Language Day", "আন্তর্জাতিক মাতৃভাষা দিবস", new DateOnly(year, 2, 21), "National");
        Add("Independence Day", "স্বাধীনতা দিবস", new DateOnly(year, 3, 26), "National");
        Add("Pohela Boishakh", "পহেলা বৈশাখ", new DateOnly(year, 4, 14), "Cultural");
        Add("May Day", "মে দিবস", new DateOnly(year, 5, 1), "National");
        Add("National Mourning Day", "জাতীয় শোক দিবস", new DateOnly(year, 8, 15), "National");
        Add("Victory Day", "বিজয় দিবস", new DateOnly(year, 12, 16), "National");
        Add("Christmas Day", "বড়দিন", new DateOnly(year, 12, 25), "Religious", "Christian");

        var eidDates = GetIslamicHolidays(year);
        foreach (var eid in eidDates)
        {
            Add(eid.Name, eid.NameBn, eid.Date, eid.Type, eid.Religion, false);
        }

        var hinduDates = GetHinduHolidays(year);
        foreach (var h in hinduDates)
        {
            Add(h.Name, h.NameBn, h.Date, h.Type, h.Religion, false);
        }

        var buddhistDates = GetBuddhistHolidays(year);
        foreach (var b in buddhistDates)
        {
            Add(b.Name, b.NameBn, b.Date, b.Type, b.Religion, false);
        }

        return holidays;
    }

    public static List<(string Name, string? NameBn, DateOnly Date, string Type, string? Religion)> GetIslamicHolidays(int year)
    {
        var result = new List<(string, string?, DateOnly, string, string?)>();

        var eidFitrApprox = GetApproximateIslamicDate(year, 10, 1);
        var eidAdhaApprox = GetApproximateIslamicDate(year, 12, 10);

        result.Add(("Eid-ul-Fitr", "ঈদ-উল-ফিতর", eidFitrApprox, "Religious", "Islam"));
        result.Add(("Eid-ul-Adha", "ঈদ-উল-আযহা", eidAdhaApprox, "Religious", "Islam"));

        var ashura = GetApproximateIslamicDate(year, 1, 10);
        result.Add(("Ashura", "আশুরা", ashura, "Religious", "Islam"));

        var eidMilad = GetApproximateIslamicDate(year, 3, 12);
        result.Add(("Eid-e-Miladunnabi", "ঈদ-ই-মিলাদুন্নবী", eidMilad, "Religious", "Islam"));

        return result;
    }

    public static List<(string Name, string? NameBn, DateOnly Date, string Type, string? Religion)> GetHinduHolidays(int year)
    {
        return new List<(string, string?, DateOnly, string, string?)>
        {
            ("Janmashtami", "জন্মাষ্টমী", GetApproximateHinduDate(year, 8, 15), "Religious", "Hindu"),
            ("Durga Puja", "দুর্গাপূজা", GetApproximateHinduDate(year, 9, 10), "Religious", "Hindu")
        };
    }

    public static List<(string Name, string? NameBn, DateOnly Date, string Type, string? Religion)> GetBuddhistHolidays(int year)
    {
        return new List<(string, string?, DateOnly, string, string?)>
        {
            ("Buddha Purnima", "বুদ্ধ পূর্ণিমা", GetApproximateBuddhistDate(year, 4, 15), "Religious", "Buddhist")
        };
    }

    private static DateOnly GetApproximateIslamicDate(int year, int month, int day)
    {
        var baseDate = new DateTime(year, month, day);
        var approx = baseDate.AddDays(-11 * (year - 1440));
        return DateOnly.FromDateTime(approx);
    }

    private static DateOnly GetApproximateHinduDate(int year, int month, int day)
    {
        return new DateOnly(year, month, Math.Min(day, DateTime.DaysInMonth(year, month)));
    }

    private static DateOnly GetApproximateBuddhistDate(int year, int month, int day)
    {
        return new DateOnly(year, month, Math.Min(day, DateTime.DaysInMonth(year, month)));
    }
}
