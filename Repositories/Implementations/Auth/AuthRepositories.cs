using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Repositories.Interfaces.Auth;

namespace SchoolManagementSystem.Repositories.Implementations.Auth;

public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
{
    public UserRepository(SchoolDbContext db) : base(db) { }
}

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(SchoolDbContext db) : base(db) { }
}

public class UserRoleRepository : BaseRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(SchoolDbContext db) : base(db) { }
}
<<<<<<< HEAD

public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(SchoolDbContext db) : base(db) { }
}
=======
>>>>>>> d8b24e6 (attendece and website curtomize)
