-- Set OpenAI API Key
-- Run this in SSMS after replacing 'YOUR_API_KEY_HERE' with your actual key
-- DO NOT commit this file with the real key

DECLARE @ApiKey NVARCHAR(500) = 'YOUR_API_KEY_HERE';

UPDATE [dbo].[AIProviders]
SET [ApiKeyEncrypted] = @ApiKey,
    [UpdatedAt] = SYSUTCDATETIME()
WHERE [Name] = 'OpenAI' AND [IsDeleted] = 0;

PRINT 'API key updated for OpenAI provider.';
GO
