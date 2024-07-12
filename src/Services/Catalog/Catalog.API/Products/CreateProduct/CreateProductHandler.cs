
using BuildingBlocks.CQRS;

using Catalog.API.Models;

namespace Catalog.API.Products.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    List<string> Categories,
    string Description,
    string ImageFile,
    decimal Price) : ICommand<CreateProductResult>;
public sealed record CreateProductResult(Guid id);


internal sealed class CreateProductHandler : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = command.Name,
            Categories = command.Categories,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price
        };

        //save to database

        return new CreateProductResult(product.Id);

    }
}
