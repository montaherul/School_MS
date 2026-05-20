using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Repositories.Interfaces.Auth;

public interface IUserRepository : IBaseRepository<ApplicationUser> { }

public interface IRoleRepository : IBaseRepository<Role> { }

public interface IUserRoleRepository : IBaseRepository<UserRole> { }
<<<<<<< HEAD
public interface IAuditLogRepository : IBaseRepository<AuditLog> { }
=======
>>>>>>> d8b24e6 (attendece and website curtomize)
