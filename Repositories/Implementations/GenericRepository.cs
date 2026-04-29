using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.Repositories.Implementations;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly SchoolDbContext _db;

    public GenericRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public IQueryable<T> Query() => _db.Set<T>().AsQueryable();

    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _db.Set<T>().FindAsync([id], cancellationToken).AsTask();

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = predicate is null ? Query() : Query().Where(predicate);
        return await query.ToListAsync(cancellationToken);
    }

    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => _db.Set<T>().AddAsync(entity, cancellationToken).AsTask();

    public void Update(T entity) => _db.Set<T>().Update(entity);

    public void Remove(T entity) => _db.Set<T>().Remove(entity);
}
