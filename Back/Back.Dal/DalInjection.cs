using Back.Dal.Repository.Commands;
using Back.Dal.Repository.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Back.Dal;

public static class DalInjection
{
    public static IServiceCollection DalInjectionModule(this IServiceCollection services)
    {
        services.AddScoped<IUserCommandsRepository, UserCommandsRepository>();
        services.AddScoped<IUserQueriesRepository, UserQueriesRepository>();
        services.AddScoped<IBookQueriesRepository, BooksQueriesRepository>();
        services.AddScoped<IBookCommandsRepository, BookCommandsRepository>();

        return services;
    }
}