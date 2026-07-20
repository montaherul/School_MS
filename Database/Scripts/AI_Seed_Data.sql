-- AI Module: Initial Seed Data
-- ============================================================

SET QUOTED_IDENTIFIER ON;
GO

-- 1. AI Providers
IF NOT EXISTS (SELECT 1 FROM [dbo].[AIProviders] WHERE [Name] = 'OpenAI' AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[AIProviders] ([Name], [ProviderType], [BaseUrl], [ApiKeyEncrypted], [IsEnabled], [Priority], [RetryCount], [TimeoutSeconds])
    VALUES (N'OpenAI', 1, N'https://api.openai.com/v1', NULL, 1, 1, 3, 60);
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[AIProviders] WHERE [Name] = 'Azure OpenAI' AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[AIProviders] ([Name], [ProviderType], [BaseUrl], [ApiKeyEncrypted], [IsEnabled], [Priority], [RetryCount], [TimeoutSeconds])
    VALUES (N'Azure OpenAI', 2, NULL, NULL, 0, 2, 3, 60);
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[AIProviders] WHERE [Name] = 'Gemini' AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[AIProviders] ([Name], [ProviderType], [BaseUrl], [ApiKeyEncrypted], [IsEnabled], [Priority], [RetryCount], [TimeoutSeconds])
    VALUES (N'Gemini', 3, NULL, NULL, 0, 3, 3, 60);
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[AIProviders] WHERE [Name] = 'Claude' AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[AIProviders] ([Name], [ProviderType], [BaseUrl], [ApiKeyEncrypted], [IsEnabled], [Priority], [RetryCount], [TimeoutSeconds])
    VALUES (N'Claude', 4, NULL, NULL, 0, 4, 3, 60);
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[AIProviders] WHERE [Name] = 'Ollama' AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[AIProviders] ([Name], [ProviderType], [BaseUrl], [ApiKeyEncrypted], [IsEnabled], [Priority], [RetryCount], [TimeoutSeconds])
    VALUES (N'Ollama', 5, NULL, NULL, 0, 5, 3, 60);
END
GO

-- 2. AI Models (GPT-4o-mini for Student/Teacher/Admin)
IF NOT EXISTS (SELECT 1 FROM [dbo].[AIModels] WHERE [Name] = 'gpt-4o-mini' AND [Role] = 1 AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[AIModels] ([Name], [ProviderId], [Role], [IsDefault], [MaxTokens], [Temperature], [IsEnabled])
    SELECT N'gpt-4o-mini', [Id], 1, 1, 2048, 0.7, 1 FROM [dbo].[AIProviders] WHERE [Name] = 'OpenAI' AND [IsDeleted] = 0;
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[AIModels] WHERE [Name] = 'gpt-4o-mini' AND [Role] = 2 AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[AIModels] ([Name], [ProviderId], [Role], [IsDefault], [MaxTokens], [Temperature], [IsEnabled])
    SELECT N'gpt-4o-mini', [Id], 2, 1, 4096, 0.7, 1 FROM [dbo].[AIProviders] WHERE [Name] = 'OpenAI' AND [IsDeleted] = 0;
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[AIModels] WHERE [Name] = 'gpt-4o-mini' AND [Role] = 3 AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[AIModels] ([Name], [ProviderId], [Role], [IsDefault], [MaxTokens], [Temperature], [IsEnabled])
    SELECT N'gpt-4o-mini', [Id], 3, 1, 4096, 0.7, 1 FROM [dbo].[AIProviders] WHERE [Name] = 'OpenAI' AND [IsDeleted] = 0;
END
GO

-- 3. AI Settings
MERGE INTO [dbo].[AISettings] AS T
USING (VALUES
    (N'AI.Enabled', N'true', N'Master switch for AI module', N'General', 1),
    (N'AI.MaintenanceMode', N'false', N'Put AI in maintenance mode', N'General', 2),
    (N'AI.DefaultProvider', N'OpenAI', N'Default LLM provider', N'General', 3),
    (N'AI.DefaultModel', N'gpt-4o-mini', N'Default model name', N'General', 4),
    (N'AI.Temperature', N'0.7', N'Default response creativity', N'General', 5),
    (N'AI.MaxTokens', N'2048', N'Max tokens per response', N'General', 6),
    (N'AI.TimeoutSeconds', N'60', N'API request timeout', N'General', 7),
    (N'AI.RetryCount', N'3', N'Number of retry attempts', N'General', 8),
    (N'AI.StreamingEnabled', N'true', N'Enable streaming responses', N'General', 9),
    (N'AI.CostPerPromptToken', N'0.00000015', N'Cost per prompt token (USD)', N'Cost', 1),
    (N'AI.CostPerCompletionToken', N'0.00000060', N'Cost per completion token (USD)', N'Cost', 2),
    (N'AI.MonthlyBudget', N'100', N'Monthly AI budget (USD)', N'Cost', 3),
    (N'AI.DailyBudget', N'5', N'Daily AI budget (USD)', N'Cost', 4)
) AS S ([Key], [Value], [Description], [Category], [DisplayOrder])
ON T.[Key] = S.[Key] AND T.[IsDeleted] = 0
WHEN NOT MATCHED THEN
    INSERT ([Key], [Value], [Description], [Category], [DisplayOrder])
    VALUES (S.[Key], S.[Value], S.[Description], S.[Category], S.[DisplayOrder]);
GO

-- 4. AI Feature Flags
MERGE INTO [dbo].[AIFeatureFlags] AS T
USING (VALUES
    (N'AI.Feature.Chat', N'Enable Chat', 1, N'Features', N'Allow students to chat with AI'),
    (N'AI.Feature.Streaming', N'Enable Streaming', 1, N'Features', N'Stream AI responses in real-time'),
    (N'AI.Feature.OCR', N'Enable OCR', 0, N'Features', N'Optical character recognition for images'),
    (N'AI.Feature.Voice', N'Enable Voice', 0, N'Features', N'Voice input/output support'),
    (N'AI.Feature.RAG', N'Enable RAG', 0, N'Features', N'Retrieval-Augmented Generation'),
    (N'AI.Feature.QuizGenerator', N'Enable Quiz Generator', 0, N'Features', N'AI-powered quiz generation'),
    (N'AI.Feature.Flashcards', N'Enable Flashcards', 0, N'Features', N'AI-generated flashcards'),
    (N'AI.Feature.HomeworkAI', N'Enable Homework AI', 0, N'Features', N'AI homework assistance'),
    (N'AI.Feature.TeacherAI', N'Enable Teacher AI', 0, N'Features', N'AI tools for teachers'),
    (N'AI.Feature.ParentAI', N'Enable Parent AI', 0, N'Features', N'AI tools for parents')
) AS S ([Key], [DisplayName], [IsEnabled], [Category], [Description])
ON T.[Key] = S.[Key] AND T.[IsDeleted] = 0
WHEN NOT MATCHED THEN
    INSERT ([Key], [DisplayName], [IsEnabled], [Category], [Description])
    VALUES (S.[Key], S.[DisplayName], S.[IsEnabled], S.[Category], S.[Description]);
GO

-- 5. AI Quotas
MERGE INTO [dbo].[AIQuotas] AS T
USING (VALUES
    (N'Student', 500, 20, 3000, 0),
    (N'Teacher', NULL, 100, NULL, 1),
    (N'Admin', NULL, NULL, NULL, 1),
    (N'Parent', 100, 10, 2000, 0)
) AS S ([Role], [DailyLimit], [MinuteLimit], [MaxTokensPerRequest], [IsUnlimited])
ON T.[Role] = S.[Role] AND T.[IsDeleted] = 0
WHEN NOT MATCHED THEN
    INSERT ([Role], [DailyLimit], [MinuteLimit], [MaxTokensPerRequest], [IsUnlimited])
    VALUES (S.[Role], S.[DailyLimit], S.[MinuteLimit], S.[MaxTokensPerRequest], S.[IsUnlimited]);
GO

-- 6. AI Security Policies
MERGE INTO [dbo].[AISecurityPolicies] AS T
USING (VALUES
    (N'Security.PromptInjectionProtection', N'true', N'Detect and block prompt injection attempts', N'General'),
    (N'Security.PIIMasking', N'true', N'Mask personally identifiable information in requests', N'General'),
    (N'Security.AllowedFileTypes', N'.txt,.pdf,.docx', N'Allowed file types for upload', N'Files'),
    (N'Security.MaxUploadSizeMB', N'10', N'Maximum file upload size in MB', N'Files'),
    (N'Security.BlockedWords', N'', N'Comma-separated list of blocked words', N'Content'),
    (N'Security.AllowedDomains', N'*', N'Allowed domains for AI requests', N'Network'),
    (N'Security.ContentModeration', N'true', N'Enable content moderation', N'Content'),
    (N'Security.ConversationRetentionDays', N'365', N'Days to retain conversations', N'Data')
) AS S ([Key], [Value], [Description], [Category])
ON T.[Key] = S.[Key] AND T.[IsDeleted] = 0
WHEN NOT MATCHED THEN
    INSERT ([Key], [Value], [Description], [Category])
    VALUES (S.[Key], S.[Value], S.[Description], S.[Category]);
GO

-- 7. AI Prompts
-- Student System Prompt
IF NOT EXISTS (SELECT 1 FROM [dbo].[AIPrompts] WHERE [Name] = 'StudentSystemPrompt' AND [IsActive] = 1 AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[AIPrompts] ([Name], [Role], [Prompt], [Version], [IsActive], [Category])
    VALUES (N'StudentSystemPrompt', N'Student',
        N'You are a helpful academic AI assistant for {{SchoolName}}.
Student: {{StudentName}} (ID: {{StudentNo}})
Class: {{ClassName}}, Section: {{SectionName}}
Group: {{GroupName}}, Academic Year: {{AcademicYear}}
Subjects: {{Subjects}}

Rules:
1. Keep responses age-appropriate and aligned with the NCTB curriculum.
2. Explain concepts step-by-step. Do NOT give direct answers to exam/homework questions.
3. Use Bengali and English as appropriate.
4. For math problems, walk through the solution method without solving completely.
5. Encourage critical thinking. Ask guiding questions.
6. Be friendly, patient, and encouraging.
7. If the student seems confused, simplify the explanation.
8. Never share personal information about the student or others.
9. If you cannot help, suggest the student ask their teacher.
10. Keep responses concise (under 200 words unless the student asks for more detail).',
        1, 1, N'System');
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[AIPrompts] WHERE [Name] = 'TitlePrompt' AND [IsActive] = 1 AND [IsDeleted] = 0)
BEGIN
    INSERT INTO [dbo].[AIPrompts] ([Name], [Role], [Prompt], [Version], [IsActive], [Category])
    VALUES (N'TitlePrompt', N'System',
        N'Generate a concise 5-8 word title for this conversation based on the user message.
User message: {{UserMessage}}
Title:',
        1, 1, N'System');
END
GO

PRINT 'AI seed data inserted successfully.';
GO
