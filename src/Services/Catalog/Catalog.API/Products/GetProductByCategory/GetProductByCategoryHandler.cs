﻿namespace Catalog.API.Products.GetProductByCategory;

public sealed record GetProductByCategoryQuery(string Category) : IQuery<GetProductByCategoryResult>;
public sealed record GetProductByCategoryResult(IEnumerable<Product> Products);

public class GetProductByCategoryQueryHandler(
    IDocumentSession session,
    ILogger<GetProductByCategoryQueryHandler> logger)
    : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
{
    public async Task<GetProductByCategoryResult> Handle(
        GetProductByCategoryQuery query,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("GetProductByCategoryQueryHandler.Handle called with {@Query}", query);

        var filteredProducts = session
            .Query<Product>()
            .Where(p => p.Categories.Contains(query.Category));

        var products = await filteredProducts.ToListAsync(cancellationToken);

        return new GetProductByCategoryResult(products);

    }
}