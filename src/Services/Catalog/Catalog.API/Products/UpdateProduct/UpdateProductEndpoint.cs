﻿namespace Catalog.API.Products.UpdateProduct;

public sealed record UpdateProductRequest(
    Guid Id,
    string Name,
    List<string> Categories,
    string Description,
    string ImageFile,
    decimal Price);

public sealed record UpdateProductResponse(bool IsSuccess);

public class UpdateProductEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/products", async (
            UpdateProductRequest request,
            ISender sender) =>
        {
            var command = request.Adapt<UpdateProductCommand>();
            var result = await sender.Send(command);

            var response = result.Adapt<UpdateProductResponse>();

            return Results.Ok(response);

        })
        .RequireAuthorization("CatalogPolicy")
        .WithName("UpdateProduct")
        .Produces<UpdateProductResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update Product")
        .WithDescription("Update Product");
    }
}
