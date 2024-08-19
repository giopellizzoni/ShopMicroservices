namespace Basket.API.Basket.DeleteBasket;

//public sealed record DeleteBasketRequest(string UserName);

public sealed record DeleteBasketResponse(bool IsSuccess);

public class DeleteBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/basket/{userName}",
            async (string userName,
                    ISender sender) =>
                {

                    var result = await sender.Send(new DeleteBasketCommand(userName));

                    var response = result.Adapt<DeleteBasketResponse>();
                    return Results.Ok(response);

                })
            .RequireAuthorization("BasketPolicy")
            .WithName("DeleteBasket")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Basket")
            .WithDescription("Delete Basket by Username");
    }
}
