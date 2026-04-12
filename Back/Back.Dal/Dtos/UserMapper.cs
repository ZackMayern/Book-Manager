namespace Back.Dal.Dtos;

public static class UserMapper
{
    public static UserDto ToDto(this User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Roles = user.Roles
    };

    public static TokenDto ToDto(this Token token) => new()
    {
        AccessToken = token.AccessToken,
        RefreshToken = token.RefreshToken,
        User = token.User.ToDto()
    };
}