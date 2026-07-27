-- Seed SSLCOMMERZ provider
IF NOT EXISTS (SELECT 1 FROM PaymentProviders WHERE Code = 'SSLCOMMERZ' AND IsDeleted = 0)
BEGIN
    INSERT INTO PaymentProviders (Code, Name, Description, [Status], IsActive, IsSandbox, Priority, SupportsRefund, SupportsSettlement, MaxRetryAttempts, SupportedCurrencies, ClassName, CreatedBy, CreatedAt, IsDeleted)
    VALUES ('SSLCOMMERZ', 'SSLCommerz', 'SSLCommerz payment gateway for Bangladesh', 1, 1, 0, 1, 1, 1, 3, 'BDT', 'SslCommerzProvider', 'System', GETUTCDATE(), 0);
    PRINT 'SSLCOMMERZ provider inserted';
END
ELSE
    PRINT 'SSLCOMMERZ provider already exists';
GO

-- Seed payment methods linked to SSLCOMMERZ
DECLARE @ProviderId INT = (SELECT Id FROM PaymentProviders WHERE Code = 'SSLCOMMERZ' AND IsDeleted = 0);
IF @ProviderId IS NOT NULL
BEGIN
    -- bKash
    IF NOT EXISTS (SELECT 1 FROM PaymentMethods WHERE Code = 'BKASH' AND PaymentProviderId = @ProviderId AND IsDeleted = 0)
        INSERT INTO PaymentMethods (Code, Name, LogoUrl, PaymentProviderId, DisplayOrder, PopularityRank, BackgroundColor, TextColor, Icon, CssClass, IsDefault, IsRecommended, IsPopular, IsActive, CreatedBy, CreatedAt, IsDeleted)
        VALUES ('BKASH', 'bKash', '/payment-logos/bkash.svg', @ProviderId, 1, 5, '#E2136E', '#FFFFFF', 'bi bi-phone', 'pm-bkash', 0, 1, 1, 1, 'System', GETUTCDATE(), 0);

    -- Nagad
    IF NOT EXISTS (SELECT 1 FROM PaymentMethods WHERE Code = 'NAGAD' AND PaymentProviderId = @ProviderId AND IsDeleted = 0)
        INSERT INTO PaymentMethods (Code, Name, LogoUrl, PaymentProviderId, DisplayOrder, PopularityRank, BackgroundColor, TextColor, Icon, CssClass, IsDefault, IsRecommended, IsPopular, IsActive, CreatedBy, CreatedAt, IsDeleted)
        VALUES ('NAGAD', 'Nagad', '/payment-logos/nagad.svg', @ProviderId, 2, 5, '#F48221', '#FFFFFF', 'bi bi-phone', 'pm-nagad', 0, 1, 1, 1, 'System', GETUTCDATE(), 0);

    -- Rocket
    IF NOT EXISTS (SELECT 1 FROM PaymentMethods WHERE Code = 'ROCKET' AND PaymentProviderId = @ProviderId AND IsDeleted = 0)
        INSERT INTO PaymentMethods (Code, Name, LogoUrl, PaymentProviderId, DisplayOrder, PopularityRank, BackgroundColor, TextColor, Icon, CssClass, IsDefault, IsRecommended, IsPopular, IsActive, CreatedBy, CreatedAt, IsDeleted)
        VALUES ('ROCKET', 'Rocket', '/payment-logos/rocket.svg', @ProviderId, 3, 3, '#7B1FA2', '#FFFFFF', 'bi bi-phone', 'pm-rocket', 0, 0, 0, 1, 'System', GETUTCDATE(), 0);

    -- Visa
    IF NOT EXISTS (SELECT 1 FROM PaymentMethods WHERE Code = 'VISA' AND PaymentProviderId = @ProviderId AND IsDeleted = 0)
        INSERT INTO PaymentMethods (Code, Name, LogoUrl, PaymentProviderId, DisplayOrder, PopularityRank, BackgroundColor, TextColor, Icon, CssClass, IsDefault, IsRecommended, IsPopular, IsActive, CreatedBy, CreatedAt, IsDeleted)
        VALUES ('VISA', 'Visa', '/payment-logos/visa.svg', @ProviderId, 4, 2, '#1A1F71', '#FFFFFF', 'bi bi-credit-card', 'pm-visa', 0, 0, 0, 1, 'System', GETUTCDATE(), 0);

    -- MasterCard
    IF NOT EXISTS (SELECT 1 FROM PaymentMethods WHERE Code = 'MASTERCARD' AND PaymentProviderId = @ProviderId AND IsDeleted = 0)
        INSERT INTO PaymentMethods (Code, Name, LogoUrl, PaymentProviderId, DisplayOrder, PopularityRank, BackgroundColor, TextColor, Icon, CssClass, IsDefault, IsRecommended, IsPopular, IsActive, CreatedBy, CreatedAt, IsDeleted)
        VALUES ('MASTERCARD', 'MasterCard', '/payment-logos/mastercard.svg', @ProviderId, 5, 2, '#EB001B', '#FFFFFF', 'bi bi-credit-card', 'pm-mastercard', 0, 0, 0, 1, 'System', GETUTCDATE(), 0);

    -- Amex
    IF NOT EXISTS (SELECT 1 FROM PaymentMethods WHERE Code = 'AMEX' AND PaymentProviderId = @ProviderId AND IsDeleted = 0)
        INSERT INTO PaymentMethods (Code, Name, LogoUrl, PaymentProviderId, DisplayOrder, PopularityRank, BackgroundColor, TextColor, Icon, CssClass, IsDefault, IsRecommended, IsPopular, IsActive, CreatedBy, CreatedAt, IsDeleted)
        VALUES ('AMEX', 'American Express', '/payment-logos/amex.svg', @ProviderId, 6, 1, '#2E6DB4', '#FFFFFF', 'bi bi-credit-card', 'pm-amex', 0, 0, 0, 1, 'System', GETUTCDATE(), 0);

    -- Internet Banking
    IF NOT EXISTS (SELECT 1 FROM PaymentMethods WHERE Code = 'INTERNET_BANKING' AND PaymentProviderId = @ProviderId AND IsDeleted = 0)
        INSERT INTO PaymentMethods (Code, Name, LogoUrl, PaymentProviderId, DisplayOrder, PopularityRank, BackgroundColor, TextColor, Icon, CssClass, IsDefault, IsRecommended, IsPopular, IsActive, CreatedBy, CreatedAt, IsDeleted)
        VALUES ('INTERNET_BANKING', 'Internet Banking', '/payment-logos/internetbanking.svg', @ProviderId, 7, 1, '#2E7D32', '#FFFFFF', 'bi bi-laptop', 'pm-internetbanking', 1, 0, 0, 1, 'System', GETUTCDATE(), 0);

    -- Upay
    IF NOT EXISTS (SELECT 1 FROM PaymentMethods WHERE Code = 'UPAY' AND PaymentProviderId = @ProviderId AND IsDeleted = 0)
        INSERT INTO PaymentMethods (Code, Name, LogoUrl, PaymentProviderId, DisplayOrder, PopularityRank, BackgroundColor, TextColor, Icon, CssClass, IsDefault, IsRecommended, IsPopular, IsActive, CreatedBy, CreatedAt, IsDeleted)
        VALUES ('UPAY', 'Upay', '/payment-logos/upay.svg', @ProviderId, 8, 0, '#00897B', '#FFFFFF', 'bi bi-phone', 'pm-upay', 0, 0, 0, 1, 'System', GETUTCDATE(), 0);

    -- CellFin
    IF NOT EXISTS (SELECT 1 FROM PaymentMethods WHERE Code = 'CELLFIN' AND PaymentProviderId = @ProviderId AND IsDeleted = 0)
        INSERT INTO PaymentMethods (Code, Name, LogoUrl, PaymentProviderId, DisplayOrder, PopularityRank, BackgroundColor, TextColor, Icon, CssClass, IsDefault, IsRecommended, IsPopular, IsActive, CreatedBy, CreatedAt, IsDeleted)
        VALUES ('CELLFIN', 'CellFin', '/payment-logos/cellfin.svg', @ProviderId, 9, 0, '#0D47A1', '#FFFFFF', 'bi bi-phone', 'pm-cellfin', 0, 0, 0, 1, 'System', GETUTCDATE(), 0);

    -- Tap
    IF NOT EXISTS (SELECT 1 FROM PaymentMethods WHERE Code = 'TAP' AND PaymentProviderId = @ProviderId AND IsDeleted = 0)
        INSERT INTO PaymentMethods (Code, Name, LogoUrl, PaymentProviderId, DisplayOrder, PopularityRank, BackgroundColor, TextColor, Icon, CssClass, IsDefault, IsRecommended, IsPopular, IsActive, CreatedBy, CreatedAt, IsDeleted)
        VALUES ('TAP', 'Tap', '/payment-logos/tap.svg', @ProviderId, 10, 0, '#6A1B9A', '#FFFFFF', 'bi bi-phone', 'pm-tap', 0, 0, 0, 1, 'System', GETUTCDATE(), 0);

    PRINT 'Payment methods seeded successfully';
END
ELSE
    PRINT 'SSLCOMMERZ provider not found - cannot seed payment methods';
GO
