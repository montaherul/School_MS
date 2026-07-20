namespace SchoolManagementSystem.Models.DTOs.AI;

public class AIKnowledgeBaseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = "text";
    public long Size { get; set; }
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AIKnowledgeBaseUpsertDto
{
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = "text";
    public string Description { get; set; } = string.Empty;
}

public class AIKnowledgeChunkDto
{
    public int Id { get; set; }
    public int KnowledgeBaseId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int ChunkIndex { get; set; }
    public int TokenCount { get; set; }
}
