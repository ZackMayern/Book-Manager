namespace Back.Api.Extensions;

public static class EndpointRouteBuilder
{
    public static void MapRouters(this IEndpointRouteBuilder app)
    {
        using IServiceScope scope = app.ServiceProvider.CreateScope();
        IEnumerable<IRouterModule> routers = scope.ServiceProvider.GetServices<IRouterModule>();

        foreach (IRouterModule router in routers)
        {
            router.MapEndpointRoutes(app);
        }
    }
}