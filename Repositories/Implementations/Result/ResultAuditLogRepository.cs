using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class ResultAuditLogRepository : BaseRepository<ResultAuditLog>, IResultAuditLogRepository
{
    public ResultAuditLogRepository(SchoolDbContext db) : base(db) { }
}
