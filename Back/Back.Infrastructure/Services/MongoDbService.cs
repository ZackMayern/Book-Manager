using Back.Domain;
using FluentResults;
using MongoDB.Driver;

namespace Back.Infrastructure.Services;

public sealed class MongoDbService(IMongoDatabase database, ILoggerService logger) : IMongoDbService
{
    private readonly IMongoDatabase _database = database ?? throw new ArgumentNullException(nameof(database));
    private readonly ILoggerService _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<List<T>> GetAsync<T>(string collectionName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));

            _logger.LogServiceInformation(collectionName, nameof(MongoDbService), nameof(GetAsync));
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            List<T> result = await collection.Find(_ => true).ToListAsync(cancellationToken);
            return result ?? [];
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "[ERR]: Operation was cancelled");
            throw;
        }
        catch (MongoConnectionException ex)
        {
            _logger.LogError(ex, "[ERR]: Database connection failed");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            throw;
        }
    }

    public async Task<T> GetByIdAsync<T>(string collectionName, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            _logger.LogServiceInformation(collectionName, nameof(MongoDbService), nameof(GetByIdAsync));
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            T result = await collection.Find(Builders<T>.Filter.Eq(Constants.Collection.Id, id)).FirstOrDefaultAsync(cancellationToken);
            return result;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "[ERR]: Operation was cancelled");
            throw;
        }
        catch (MongoConnectionException ex)
        {
            _logger.LogError(ex, "[ERR]: Database connection failed");
            throw;
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            throw;
        }
    }

    public async Task<T> GetByEmailAsync<T>(string collectionName, string email, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email));

            _logger.LogServiceInformation(collectionName, nameof(MongoDbService), nameof(GetByEmailAsync));
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            T result = await collection.Find(Builders<T>.Filter.Eq(Constants.Collection.Email, email)).FirstOrDefaultAsync(cancellationToken);
            return result;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "[ERR]: Operation was cancelled");
            throw;
        }
        catch (MongoConnectionException ex)
        {
            _logger.LogError(ex, "[ERR]: Database connection failed");
            throw;
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            throw;
        }
    }

    public async Task<Result<DatabaseEventType>> CreateAsync<T>(string collectionName, T document, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            _logger.LogServiceInformation(collectionName, nameof(MongoDbService), nameof(CreateAsync));
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            await collection.InsertOneAsync(document, cancellationToken: cancellationToken);
            return DatabaseEventType.RowCreated;
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            return Result.Fail($"{ex.Message}");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "[ERR]: Operation was cancelled");
            return Result.Fail("Operation was cancelled");
        }
        catch (MongoConnectionException ex)
        {
            _logger.LogError(ex, "[ERR]: Database connection failed");
            return Result.Fail("Database connection failed");
        }
        catch (MongoWriteException ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            return Result.Fail($"{ex.Message}");
        }
        catch (MongoServerException ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            return Result.Fail($"Server error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            return Result.Fail($"{ex.Message}");
        }
    }

    public async Task<Result<DatabaseEventType>> UpdateAsync<T>(string collectionName, string id, T document, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            _logger.LogServiceInformation(collectionName, nameof(MongoDbService), nameof(UpdateAsync));
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            ReplaceOneResult result = await collection.ReplaceOneAsync(Builders<T>.Filter.Eq(Constants.Collection.Id, id), document, cancellationToken: cancellationToken);
            return result.IsAcknowledged ? DatabaseEventType.RowUpdated : Result.Fail("Row not updated");
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            return Result.Fail($"{ex.Message}");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "[ERR]: Operation was cancelled");
            return Result.Fail("Operation was cancelled");
        }
        catch (MongoConnectionException ex)
        {
            _logger.LogError(ex, "[ERR]: Database connection failed");
            return Result.Fail("Database connection failed");
        }
        catch (MongoWriteException ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            return Result.Fail($"{ex.Message}");
        }
        catch (MongoServerException ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            return Result.Fail($"Server error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            return Result.Fail($"{ex.Message}");
        }
    }

    public async Task<Result<DatabaseEventType>> DeleteAsync<T>(string collectionName, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            _logger.LogServiceInformation(collectionName, nameof(MongoDbService), nameof(DeleteAsync));
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            DeleteResult result = await collection.DeleteOneAsync(Builders<T>.Filter.Eq(Constants.Collection.Id, id), cancellationToken: cancellationToken);
            return result.IsAcknowledged ? DatabaseEventType.RowDeleted : Result.Fail("Row was not deleted");
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            return Result.Fail($"{ex.Message}");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "[ERR]: Operation was cancelled");
            return Result.Fail("Operation was cancelled");
        }
        catch (MongoConnectionException ex)
        {
            _logger.LogError(ex, "[ERR]: Database connection failed");
            return Result.Fail("Database connection failed");
        }
        catch (MongoServerException ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            return Result.Fail($"Server error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ERR]: {ex.Message}");
            return Result.Fail($"{ex.Message}");
        }
    }
}
