using SchoolManagementSystem.Data;
using SchoolManagementSystem.Repositories.Implementations;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.UnitOfWork.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly SchoolDbContext _db;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(SchoolDbContext db)
    {
        _db = db;
    }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new GenericRepository<T>(_db);
        }

        return (IGenericRepository<T>)_repositories[type];
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
