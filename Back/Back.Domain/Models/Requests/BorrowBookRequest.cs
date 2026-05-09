namespace Back.Domain.Models.Requests;

public sealed class BorrowBookRequest
{
    public string BookId { get; set; } = string.Empty;
    public int BorrowDays { get; set; } = 14;
}