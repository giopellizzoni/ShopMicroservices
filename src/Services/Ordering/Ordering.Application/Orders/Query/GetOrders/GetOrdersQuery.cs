using BuildingBlocks.Pagination;

namespace Ordering.Application.Orders.Query.GetOrders;

public record GetOrdersQuery(PaginationRequest Request) : IQuery<GetOrdersQueryResult>;

public record GetOrdersQueryResult(PaginatedResult<OrderDto> OrderDtos);
