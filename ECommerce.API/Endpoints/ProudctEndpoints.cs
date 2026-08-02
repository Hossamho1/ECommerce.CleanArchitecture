//namespace ECommerce.API.Endpoints;

//public static class ProudctEndpoints
//{
//    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
//    {
//        var group = app.MapGroup("/api/products").WithTags("Products");
//        group.MapGet("/", GetAllProducts)
//            .Produces<GetAllProductsResponse>(StatusCodes.Status200OK)
//            .WithName("GetAllProducts")
//            .WithSummary("Get all products");
//        group.MapGet("/{id:guid}", GetByIdProduct)
//            .Produces<GetByIdProductResponse>(StatusCodes.Status200OK)
//            .Produces(StatusCodes.Status404NotFound)
//            .WithName("GetByIdProduct")
//            .WithSummary("Get product by id");
//        return app;
//    }

//}
