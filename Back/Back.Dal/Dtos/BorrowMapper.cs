namespace Back.Dal.Dtos;

public static class BorrowMapper
{
    public static BorrowRecordDto ToDto(this BorrowRecord record, User? user = null, Book? book = null) => new()
    {
        Id = record.Id,
        UserId = record.UserId,
        UserFullName = user == null ? string.Empty : $"{user.FirstName} {user.LastName}".Trim(),
        BookId = record.BookId,
        BookTitle = book?.Title ?? string.Empty,
        BorrowedAt = record.BorrowedAt,
        DueDate = record.DueDate,
        ReturnedAt = record.ReturnedAt,
        Status = record.Status,
        RenewCount = record.RenewCount
    };

    public static BookRequestDto ToDto(this BookRequest request, User? user = null) => new()
    {
        Id = request.Id,
        UserId = request.UserId,
        UserFullName = user == null ? string.Empty : $"{user.FirstName} {user.LastName}".Trim(),
        Title = request.Title,
        Author = request.Author,
        Reason = request.Reason,
        Status = request.Status,
        AdminMessage = request.AdminMessage,
        CreatedAt = request.CreatedAt
    };
}