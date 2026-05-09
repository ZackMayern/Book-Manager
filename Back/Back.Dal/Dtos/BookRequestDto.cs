namespace Back.Dal.Dtos;

public sealed class BookRequestDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AdminMessage { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}