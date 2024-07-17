namespace Catalog.API.Products.GetProductByCategory;

public sealed record GetProductByCategoryQuery(string Category) : IQuery<GetProductByCategoryResult>;
public sealed record GetProductByCategoryResult(IEnumerable<Product> Products);

public class GetProductByCategoryQueryHandler(
    IDocumentSession session)
    : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
{
    public async Task<GetProductByCategoryResult> Handle(
        GetProductByCategoryQuery query,
        CancellationToken cancellationToken)
    {
        var filteredProducts = session
            .Query<Product>()
            .Where(p => p.Categories.Contains(query.Category));

        var products = await filteredProducts.ToListAsync(cancellationToken);

        return new GetProductByCategoryResult(products);
    }
}
