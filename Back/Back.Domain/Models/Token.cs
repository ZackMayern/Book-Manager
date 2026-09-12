namespace Back.Domain.Models;

using System.Text.Json.Serialization;

public sealed class Token
{
    public string AccessToken { get; set; } = string.Empty;

    [JsonIgnore]
    public string RefreshToken { get; set; } = string.Empty;
    public User User { get; set; } = new();
}