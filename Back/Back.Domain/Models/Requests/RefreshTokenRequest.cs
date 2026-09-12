using System.Text.Json.Serialization;

namespace Back.Domain.Models.Requests;

public sealed class RefreshTokenRequest
{
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
}