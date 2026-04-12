using Supabase.Postgrest.Models;

namespace Back.Domain.Services.Abstractions;

public interface ISupabaseClientService
{
    Task<List<T>> GetAllAsync<T>(string tableName, CancellationToken cancellationToken = default)
        where T : BaseModel, new();

    Task<T?> GetByIdAsync<T>(string tableName, string id, CancellationToken cancellationToken = default)
        where T : BaseModel, new();

    Task<T?> GetByEmailAsync<T>(string tableName, string email, CancellationToken cancellationToken = default)
        where T : BaseModel, new();

    Task<DatabaseEventType> CreateAsync<T>(
        string tableName,
        T entity,
        CancellationToken cancellationToken = default)
        where T : BaseModel, new();

    Task<DatabaseEventType> UpdateAsync<T>(
        string tableName,
        string id,
        T entity,
        CancellationToken cancellationToken = default)
        where T : BaseModel, new();

    Task<DatabaseEventType> DeleteAsync<T>(
        string tableName,
        string id,
        CancellationToken cancellationToken = default)
        where T : BaseModel, new();
}