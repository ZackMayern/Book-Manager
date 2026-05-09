using Back.Domain.Logic.Commands;
using Back.Domain.Logic.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Back.Domain;

public static class DomainInjection
{
    public static IServiceCollection DomainInjectionModule(this IServiceCollection services)
    {
        services.AddScoped<IUserCommandsDomain, UserCommandsDomain>();
        services.AddScoped<IUserQueriesDomain, UserQueriesDomain>();
        services.AddScoped<IAuthDomain, AuthDomain>();
        services.AddScoped<IBookQueriesDomain, BookQueriesDomain>();
        services.AddScoped<IBookCommandsDomain, BookCommandsDomain>();
        services.AddScoped<IBorrowQueriesDomain, BorrowQueriesDomain>();
        services.AddScoped<IBorrowCommandsDomain, BorrowCommandsDomain>();
        services.AddScoped<IBookRequestQueriesDomain, BookRequestQueriesDomain>();
        services.AddScoped<IBookRequestCommandsDomain, BookRequestCommandsDomain>();

        return services;
    }
}