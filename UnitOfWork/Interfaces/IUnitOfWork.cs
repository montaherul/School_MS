using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.UnitOfWork.Interfaces;

public interface IUnitOfWork
{
    IBaseRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
