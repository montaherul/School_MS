using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.SchoolPay;

namespace SchoolManagementSystem.Data;

public static class PaymentMethodSeeder
{
    public static async Task SeedAsync(SchoolDbContext db)
    {
        var sslProvider = await db.Set<PaymentProvider>()
            .FirstOrDefaultAsync(p => p.Code == "SSLCOMMERZ" && !p.IsDeleted);

        if (sslProvider == null) return;

        var existingCodes = await db.Set<PaymentMethod>()
            .Where(m => m.PaymentProviderId == sslProvider.Id && !m.IsDeleted)
            .Select(m => m.Code)
            .ToListAsync();

        var seedMethods = new[]
        {
            new
            {
                Code = "BKASH",
                Name = "bKash",
                LogoUrl = "/payment-logos/bkash.svg",
                DisplayOrder = 1,
                PopularityRank = 5,
                BackgroundColor = "#E2136E",
                TextColor = "#FFFFFF",
                Icon = "bi bi-phone",
                CssClass = "pm-bkash",
                IsDefault = false,
                IsRecommended = true,
                IsPopular = true
            },
            new
            {
                Code = "NAGAD",
                Name = "Nagad",
                LogoUrl = "/payment-logos/nagad.svg",
                DisplayOrder = 2,
                PopularityRank = 5,
                BackgroundColor = "#F48221",
                TextColor = "#FFFFFF",
                Icon = "bi bi-phone",
                CssClass = "pm-nagad",
                IsDefault = false,
                IsRecommended = true,
                IsPopular = true
            },
            new
            {
                Code = "ROCKET",
                Name = "Rocket",
                LogoUrl = "/payment-logos/rocket.svg",
                DisplayOrder = 3,
                PopularityRank = 3,
                BackgroundColor = "#7B1FA2",
                TextColor = "#FFFFFF",
                Icon = "bi bi-phone",
                CssClass = "pm-rocket",
                IsDefault = false,
                IsRecommended = false,
                IsPopular = false
            },
            new
            {
                Code = "VISA",
                Name = "Visa",
                LogoUrl = "/payment-logos/visa.svg",
                DisplayOrder = 4,
                PopularityRank = 2,
                BackgroundColor = "#1A1F71",
                TextColor = "#FFFFFF",
                Icon = "bi bi-credit-card",
                CssClass = "pm-visa",
                IsDefault = false,
                IsRecommended = false,
                IsPopular = false
            },
            new
            {
                Code = "MASTERCARD",
                Name = "MasterCard",
                LogoUrl = "/payment-logos/mastercard.svg",
                DisplayOrder = 5,
                PopularityRank = 2,
                BackgroundColor = "#EB001B",
                TextColor = "#FFFFFF",
                Icon = "bi bi-credit-card",
                CssClass = "pm-mastercard",
                IsDefault = false,
                IsRecommended = false,
                IsPopular = false
            },
            new
            {
                Code = "AMEX",
                Name = "American Express",
                LogoUrl = "/payment-logos/amex.svg",
                DisplayOrder = 6,
                PopularityRank = 1,
                BackgroundColor = "#2E6DB4",
                TextColor = "#FFFFFF",
                Icon = "bi bi-credit-card",
                CssClass = "pm-amex",
                IsDefault = false,
                IsRecommended = false,
                IsPopular = false
            },
            new
            {
                Code = "INTERNET_BANKING",
                Name = "Internet Banking",
                LogoUrl = "/payment-logos/internetbanking.svg",
                DisplayOrder = 7,
                PopularityRank = 1,
                BackgroundColor = "#2E7D32",
                TextColor = "#FFFFFF",
                Icon = "bi bi-laptop",
                CssClass = "pm-internetbanking",
                IsDefault = true,
                IsRecommended = false,
                IsPopular = false
            },
            new
            {
                Code = "UPAY",
                Name = "Upay",
                LogoUrl = "/payment-logos/upay.svg",
                DisplayOrder = 8,
                PopularityRank = 0,
                BackgroundColor = "#00897B",
                TextColor = "#FFFFFF",
                Icon = "bi bi-phone",
                CssClass = "pm-upay",
                IsDefault = false,
                IsRecommended = false,
                IsPopular = false
            },
            new
            {
                Code = "CELLFIN",
                Name = "CellFin",
                LogoUrl = "/payment-logos/cellfin.svg",
                DisplayOrder = 9,
                PopularityRank = 0,
                BackgroundColor = "#0D47A1",
                TextColor = "#FFFFFF",
                Icon = "bi bi-phone",
                CssClass = "pm-cellfin",
                IsDefault = false,
                IsRecommended = false,
                IsPopular = false
            },
            new
            {
                Code = "TAP",
                Name = "Tap",
                LogoUrl = "/payment-logos/tap.svg",
                DisplayOrder = 10,
                PopularityRank = 0,
                BackgroundColor = "#6A1B9A",
                TextColor = "#FFFFFF",
                Icon = "bi bi-phone",
                CssClass = "pm-tap",
                IsDefault = false,
                IsRecommended = false,
                IsPopular = false
            }
        };

        foreach (var s in seedMethods)
        {
            if (existingCodes.Contains(s.Code)) continue;

            db.Set<PaymentMethod>().Add(new PaymentMethod
            {
                Code = s.Code,
                Name = s.Name,
                LogoUrl = s.LogoUrl,
                PaymentProviderId = sslProvider.Id,
                DisplayOrder = s.DisplayOrder,
                PopularityRank = s.PopularityRank,
                BackgroundColor = s.BackgroundColor,
                TextColor = s.TextColor,
                Icon = s.Icon,
                CssClass = s.CssClass,
                IsDefault = s.IsDefault,
                IsRecommended = s.IsRecommended,
                IsPopular = s.IsPopular,
                IsActive = true,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();
    }
}
