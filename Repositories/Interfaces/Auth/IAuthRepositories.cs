using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Repositories.Interfaces.Auth;

public interface IUserRepository : IBaseRepository<ApplicationUser> { }

public interface IRoleRepository : IBaseRepository<Role> { }

public interface IUserRoleRepository : IBaseRepository<UserRole> { }
public interface IAuditLogRepository : IBaseRepository<AuditLog> { }
