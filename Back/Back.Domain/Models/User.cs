using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Back.Domain.Models;

public sealed record User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string[] Roles { get; init; } = ["User"];

    [JsonIgnore]
    public string PasswordHash { get; init; } = string.Empty;

    [JsonIgnore]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonIgnore]
    public DateTime RefreshTokenExpiry { get; set; }
}