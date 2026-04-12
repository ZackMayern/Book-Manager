using Supabase.Postgrest.Models;
using SupabaseClient = Supabase.Client;

namespace Back.Infrastructure.Services;

public sealed class SupabaseClientService(SupabaseClient supabaseClient, ILoggerService logger) : ISupabaseClientService
{
    private readonly SupabaseClient _supabaseClient = supabaseClient ?? throw new ArgumentNullException(nameof(supabaseClient));
    private readonly ILoggerService _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<List<T>> GetAllAsync<T>(string tableName, CancellationToken cancellationToken = default)
        where T : BaseModel, new()
    {
        ValidateTableName(tableName);
        _logger.LogServiceInformation(tableName, nameof(SupabaseClientService), nameof(GetAllAsync));

        var response = await _supabaseClient
            .From<T>()
            .Get(cancellationToken);

        return response.Models;
    }

    public async Task<T?> GetByIdAsync<T>(string tableName, string id, CancellationToken cancellationToken = default)
        where T : BaseModel, new()
    {
        ValidateTableName(tableName);
        ValidateId(id);
        _logger.LogServiceInformation(tableName, nameof(SupabaseClientService), nameof(GetByIdAsync));

        var response = await _supabaseClient
            .From<T>()
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id)
            .Single(cancellationToken);

        return response;
    }

    public async Task<T?> GetByEmailAsync<T>(string tableName, string email, CancellationToken cancellationToken = default)
        where T : BaseModel, new()
    {
        ValidateTableName(tableName);
        ValidateEmail(email);
        _logger.LogServiceInformation(tableName, nameof(SupabaseClientService), nameof(GetByEmailAsync));

        var response = await _supabaseClient
            .From<T>()
            .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
            .Single(cancellationToken);

        return response;
    }

    public async Task<DatabaseEventType> CreateAsync<T>(
        string tableName,
        T entity,
        CancellationToken cancellationToken = default)
        where T : BaseModel, new()
    {
        ValidateTableName(tableName);
        ValidateDocument(entity);
        _logger.LogServiceInformation(tableName, nameof(SupabaseClientService), nameof(CreateAsync));

        await _supabaseClient
            .From<T>()
            .Insert(entity, cancellationToken: cancellationToken);

        return DatabaseEventType.RowCreated;
    }

    public async Task<DatabaseEventType> UpdateAsync<T>(
        string tableName,
        string id,
        T entity,
        CancellationToken cancellationToken = default)
        where T : BaseModel, new()
    {
        ValidateTableName(tableName);
        ValidateId(id);
        ValidateDocument(entity);
        _logger.LogServiceInformation(tableName, nameof(SupabaseClientService), nameof(UpdateAsync));

        await _supabaseClient
            .From<T>()
            .Filter("Id", Supabase.Postgrest.Constants.Operator.Equals, id)
            .Update(entity, cancellationToken: cancellationToken);

        return DatabaseEventType.RowUpdated;
    }

    public async Task<DatabaseEventType> DeleteAsync<T>(
        string tableName,
        string id,
        CancellationToken cancellationToken = default)
        where T : BaseModel, new()
    {
        ValidateTableName(tableName);
        ValidateId(id);
        _logger.LogServiceInformation(tableName, nameof(SupabaseClientService), nameof(DeleteAsync));

        await _supabaseClient
            .From<T>()
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id)
            .Delete(cancellationToken: cancellationToken);

        return DatabaseEventType.RowDeleted;
    }

    private static void ValidateTableName(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be null or empty", nameof(tableName));
    }

    private static void ValidateId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));
    }

    private static void ValidateDocument<T>(T document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document), "Document cannot be null");
    }
}
