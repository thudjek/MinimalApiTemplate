namespace MinimalApiTemplate.REST.Common;

public interface IEndpoint
{
    public static abstract void MapEndpoint(IEndpointRouteBuilder app);
}