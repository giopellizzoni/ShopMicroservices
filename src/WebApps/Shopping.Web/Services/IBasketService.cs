namespace Shopping.Web.Services;

public interface IBasketService
{
    [Get("/basket-service/basket/{username}")]
    Task<GetBasketResponse> GetBasket(string userName);

    [Post("/basket-service/basket")]
    Task<StoreBasketResponse> CreateBasket(StoreBasketRequest request);

    [Delete("/basket-service/basket/{username}")]
    Task<DeleteBasketResponse> DeleteBasket(string userName);

    [Post("/basket-service/basket/checkout")]
    Task<CheckoutBasketResponse> CheckoutBasket(CheckoutBasketRequest request);
}
