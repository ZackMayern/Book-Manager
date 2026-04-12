namespace Back.Api.Endpoints.Abstractions
{
    public interface IRouterModule
    {
        void MapEndpointRoutes(IEndpointRouteBuilder app);
    }
}
