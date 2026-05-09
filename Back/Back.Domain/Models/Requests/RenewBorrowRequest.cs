namespace Back.Domain.Models.Requests;

public sealed class RenewBorrowRequest
{
    public int ExtendDays { get; set; } = 7;
}