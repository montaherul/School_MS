using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Repositories.Interfaces;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace SchoolManagementSystem.Repositories.Implementations;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly SchoolDbContext _db;
    protected readonly DbSet<T> _set;

    public BaseRepository(SchoolDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public IQueryable<T> Query() => _set.AsQueryable();

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _set.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _set.AsQueryable();
        if (predicate != null) query = query.Where(predicate);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _set.FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _set.AnyAsync(predicate, cancellationToken);

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => predicate != null ? await _set.CountAsync(predicate, cancellationToken) : await _set.CountAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _set.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        => await _set.AddRangeAsync(entities, cancellationToken);

    public void Update(T entity) => _set.Update(entity);

    public void Remove(T entity) => _set.Remove(entity);

    public void RemoveRange(IEnumerable<T> entities) => _set.RemoveRange(entities);

    // Stored procedure helpers (eliminate duplication across all repository implementations)
    protected static async Task<IAsyncDisposable> OpenConnectionAsync(DbConnection connection, CancellationToken ct)
    {
        var wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed) await connection.OpenAsync(ct);
        return new ConnectionLease(connection, wasClosed);
    }

    protected static void AddParameter(DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }

    protected static string GetString(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? string.Empty : Convert.ToString(reader[name]) ?? string.Empty;
    protected static string? GetNullableString(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToString(reader[name]);
    protected static int GetInt32(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? 0 : Convert.ToInt32(reader[name]);
    protected static int? GetNullableInt32(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToInt32(reader[name]);
    protected static decimal GetDecimal(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? 0m : Convert.ToDecimal(reader[name]);
    protected static decimal? GetNullableDecimal(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToDecimal(reader[name]);
    protected static bool GetBoolean(DbDataReader reader, string name) => !reader.IsDBNull(reader.GetOrdinal(name)) && Convert.ToBoolean(reader[name]);
    protected static bool? GetNullableBoolean(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToBoolean(reader[name]);
    protected static DateTime GetDateTime(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? DateTime.MinValue : Convert.ToDateTime(reader[name]);
    protected static DateTime? GetNullableDateTime(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToDateTime(reader[name]);
    protected static DateOnly? GetNullableDateOnly(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal(name)));

    private sealed class ConnectionLease : IAsyncDisposable
    {
        private readonly DbConnection _connection;
        private readonly bool _closeOnDispose;
        public ConnectionLease(DbConnection connection, bool closeOnDispose) { _connection = connection; _closeOnDispose = closeOnDispose; }
        public async ValueTask DisposeAsync() { if (_closeOnDispose) await _connection.CloseAsync(); }
    }
}