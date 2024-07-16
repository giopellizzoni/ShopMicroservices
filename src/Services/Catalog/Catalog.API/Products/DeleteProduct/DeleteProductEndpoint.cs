namespace Catalog.API.Products.DeleteProduct;

//public sealed record DeleteProductRequest();

public sealed record DeleteProductResponse(bool IsSuccess);

public class DeleteProductEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/products/{id}", async (
            Guid id,
            ISender sender) =>
        {
            var result = await sender.Send(new DeleteProductCommand(id));
            var response = result.Adapt<DeleteProductResponse>();
            return Results.Ok(response);
        }).WithName("DeleteProduct")
        .Produces<DeleteProductResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Delete product")
        .WithDescription("Delete product");
    }
}
