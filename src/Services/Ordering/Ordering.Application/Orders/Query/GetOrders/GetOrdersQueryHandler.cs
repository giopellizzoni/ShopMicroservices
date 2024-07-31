using BuildingBlocks.Pagination;

namespace Ordering.Application.Orders.Query.GetOrders;
public class GetOrdersQueryHandler(IApplicationDbContext context) : IQueryHandler<GetOrdersQuery, GetOrdersQueryResult>
{
    public async Task<GetOrdersQueryResult> Handle(
        GetOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var pageIndex = query.Request.PageIndex;
        var pageSize = query.Request.PageSize;
        var totalCount = await context.OrderItems.LongCountAsync(cancellationToken);

        var orders = await context.Orders
            .Include(o => o.OrderItems)
            .OrderBy(o => o.OrderName.Value)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new GetOrdersQueryResult(
            new PaginatedResult<OrderDto>(
                pageIndex,
                pageSize,
                totalCount,
                orders.ToOrderDtoList()));
    }
}
