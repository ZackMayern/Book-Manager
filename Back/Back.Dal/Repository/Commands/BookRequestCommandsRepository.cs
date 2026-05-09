using Back.Domain;

namespace Back.Dal.Repository.Commands;

public sealed class BookRequestCommandsRepository(ISupabaseClientService supabaseClientService) : IBookRequestCommandsRepository
{
    private readonly ISupabaseClientService _supabaseClientService =
        supabaseClientService ?? throw new ArgumentNullException(nameof(supabaseClientService));

    public async Task<Result<DatabaseEventType>> AddAsync(BookRequest request, CancellationToken cancellationToken = default)
    {
        Result<DatabaseEventType> result =
            await _supabaseClientService.CreateAsync(Constants.Collection.BookRequests, request, cancellationToken);
        return result;
    }

    public async Task<Result<DatabaseEventType>> UpdateAsync(BookRequest request, CancellationToken cancellationToken = default)
    {
        Result<DatabaseEventType> result =
            await _supabaseClientService.UpdateAsync(Constants.Collection.BookRequests, request.Id, request, cancellationToken);
        return result;
    }
}