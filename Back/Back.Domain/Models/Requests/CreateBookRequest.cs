namespace Back.Domain.Models.Requests;

public sealed class CreateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}