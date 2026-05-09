namespace Back.Domain.Models.Requests;

public sealed class UpdateBorrowStatusRequest
{
    public string Status { get; set; } = string.Empty;
}