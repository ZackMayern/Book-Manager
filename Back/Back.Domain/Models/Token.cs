namespace Back.Domain.Models;

public sealed class Token
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public User User { get; set; } = new();
}