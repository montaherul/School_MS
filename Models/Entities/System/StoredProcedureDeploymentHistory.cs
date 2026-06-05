using System;

namespace SchoolManagementSystem.Models.Entities.System
{
    public class StoredProcedureDeploymentHistory
    {
        public int Id { get; set; }
        public string ProcedureName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public DateTime DeployedAt { get; set; }
        public string Status { get; set; } = string.Empty; // "Success" or "Failed"
        public string? ErrorMessage { get; set; }
    }
}
