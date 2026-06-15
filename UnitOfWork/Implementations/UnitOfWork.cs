using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Repositories.Implementations;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Reflection;

namespace SchoolManagementSystem.UnitOfWork.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly SchoolDbContext _db;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(SchoolDbContext db, IServiceProvider serviceProvider)
    {
        _db = db;
        _serviceProvider = serviceProvider;
    }

    public IBaseRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (_repositories.ContainsKey(type))
        {
            return (IBaseRepository<T>)_repositories[type];
        }

        // Try to find a specialized repository first (e.g., IStudentRepo)
        // We look for an interface that inherits from IBaseRepository<T>
        // and only inspect application assemblies so framework assembly load issues
        // do not break repository resolution.
        var specializedRepoType = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => IsApplicationAssembly(assembly))
            .SelectMany(GetLoadableTypes)
            .FirstOrDefault(p => typeof(IBaseRepository<T>).IsAssignableFrom(p) && p.IsInterface && p != typeof(IBaseRepository<T>));

        if (specializedRepoType != null)
        {
            var repo = _serviceProvider.GetService(specializedRepoType);
            if (repo != null)
            {
                _repositories[type] = repo;
                return (IBaseRepository<T>)repo;
            }
        }

        var repositoryInstance = new BaseRepository<T>(_db);
        _repositories[type] = repositoryInstance;
        return repositoryInstance;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
    public Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters) => _db.Database.ExecuteSqlRawAsync(sql, parameters);
    public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
    {
        var strategy = _db.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await BeginTransactionAsync(cancellationToken);

            try
            {
                await operation();
                await CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _db.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_db.Database.CurrentTransaction != null)
            await _db.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_db.Database.CurrentTransaction != null)
            await _db.Database.RollbackTransactionAsync(cancellationToken);
    }

    private static bool IsApplicationAssembly(Assembly assembly)
    {
        var name = assembly.GetName().Name;
        return !string.IsNullOrWhiteSpace(name) && name.StartsWith("SchoolManagementSystem", StringComparison.Ordinal);
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(type => type != null)!;
        }
    }
}
