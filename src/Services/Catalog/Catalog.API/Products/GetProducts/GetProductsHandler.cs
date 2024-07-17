namespace Catalog.API.Products.GetProducts;

public sealed record GetProductsQuery : IQuery<GetProductsResult>;
public sealed record GetProductsResult(IEnumerable<Product> Products);

internal sealed class GetProductsQueryHandler(IDocumentSession session)
    : IQueryHandler<GetProductsQuery, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken)
    {

        var products = await session.Query<Product>().ToListAsync(cancellationToken);

        return new GetProductsResult(products);
    }

}
