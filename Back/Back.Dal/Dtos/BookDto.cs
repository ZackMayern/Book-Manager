namespace Back.Dal.Dtos;

public sealed class BookDto
{
    public string Id { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string YearOfPublication { get; set; } = string.Empty;
    public bool Availability { get; set; } = true;
    public string Condition { get; set; } = string.Empty;
}
