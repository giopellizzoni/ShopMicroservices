﻿using Ordering.Application.Orders.Commands.DeleteOrder;

namespace Ordering.API.Endpoints;

// public record DeleteOrderRequest(Guid Id);
public record DeleteOrderResponse(bool IsSuccess);


public class DeleteOrder : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/orders/{id}", async (Guid Id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteOrderCommand(Id));

                var response = result.Adapt<DeleteOrderResponse>();

                return Results.Ok(response);
            })
            .RequireAuthorization("OrderingPolicy")
            .WithName("DeleteOrder")
            .Produces<DeleteOrderResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Order")
            .WithDescription("Delete an existing order");
    }
}
