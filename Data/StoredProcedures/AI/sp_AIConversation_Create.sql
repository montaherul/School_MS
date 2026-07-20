CREATE PROCEDURE [dbo].[sp_AIConversation_Create]
    @StudentId INT,
    @Title NVARCHAR(200),
    @CreatedBy NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[AIConversations] ([StudentId], [Title], [CreatedBy])
    VALUES (@StudentId, @Title, @CreatedBy);

    SELECT SCOPE_IDENTITY() AS [Id];
END
