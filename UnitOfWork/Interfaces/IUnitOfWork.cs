using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.UnitOfWork.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
