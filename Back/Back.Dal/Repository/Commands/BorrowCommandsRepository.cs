using Back.Domain;

namespace Back.Dal.Repository.Commands;

public sealed class BorrowCommandsRepository(ISupabaseClientService supabaseClientService) : IBorrowCommandsRepository
{
    private readonly ISupabaseClientService _supabaseClientService =
        supabaseClientService ?? throw new ArgumentNullException(nameof(supabaseClientService));

    public async Task<Result<DatabaseEventType>> AddAsync(BorrowRecord record, CancellationToken cancellationToken = default)
    {
        Result<DatabaseEventType> result =
            await _supabaseClientService.CreateAsync(Constants.Collection.BorrowRecords, record, cancellationToken);
        return result;
    }

    public async Task<Result<DatabaseEventType>> UpdateAsync(BorrowRecord record, CancellationToken cancellationToken = default)
    {
        Result<DatabaseEventType> result =
            await _supabaseClientService.UpdateAsync(Constants.Collection.BorrowRecords, record.Id, record, cancellationToken);
        return result;
    }
}