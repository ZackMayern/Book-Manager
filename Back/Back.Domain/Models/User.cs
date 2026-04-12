using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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

    public string PasswordHash { get; init; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime RefreshTokenExpiry { get; set; }
}