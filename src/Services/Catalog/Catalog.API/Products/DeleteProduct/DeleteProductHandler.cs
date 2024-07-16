namespace Catalog.API.Products.DeleteProduct;


public sealed record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;

public sealed record DeleteProductResult(bool IsSuccess);

public class DeleteProductCommandHandler(IDocumentSession session, ILogger<DeleteProductCommandHandler> logger): ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    public async Task<DeleteProductResult> Handle(
        DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("DeleteProductCommandHandler.Handle called with {@Command}", command);
        var product = await session.LoadAsync<Product>(command.Id, cancellationToken);
        if (product is null) throw new ProductNotFoundException();

        session.Delete(product);
        await session.SaveChangesAsync(cancellationToken);
        return new DeleteProductResult(true);
    }
}
