using Back.Helpers;
using FluentResults;

namespace Back.Services.Abstractions
{
    public interface IMongoDBService
    {
        Task<List<T>> GetAsync<T>(string collectionName, CancellationToken cancellationToken = default);
        Task<T> GetByIdAsync<T>(string collectionName, string id, CancellationToken cancellationToken = default);
        Task<T> GetByEmailAsync<T>(string collectionName, string email, CancellationToken cancellationToken = default);
        Task<Result<DatabaseEventType>> CreateAsync<T>(string collectionName, T document, CancellationToken cancellationToken = default);
        Task<Result<DatabaseEventType>> UpdateAsync<T>(string collectionName, string id, T document, CancellationToken cancellationToken = default);
        Task<Result<DatabaseEventType>> DeleteAsync<T>(string collectionName, string id, CancellationToken cancellationToken = default);
    }
}
