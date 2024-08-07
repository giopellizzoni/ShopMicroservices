using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shopping.Web.Pages;

public class CheckoutModel(IBasketService basketService, ILogger<CheckoutModel> logger) : PageModel
{
    public ShoppingCartModel Cart { get; set; } = default!;

    [BindProperty]
    public BasketCheckoutModel Order { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        Cart = await basketService.LoadUserBasket();

        return Page();
    }

    public async Task<IActionResult> OnPostCheckOutAsync()
    {
        logger.LogInformation("Checkout button clicked");

        Cart = await basketService.LoadUserBasket();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Order.CustomerId = new Guid("58C49479-EC65-4DE2-86E7-033C546291AA");
        Order.UserName = Cart.UserName;
        Order.TotalPrice = Cart.TotalPrice;

        await basketService.CheckoutBasket(new CheckoutBasketRequest(Order));

        return RedirectToPage("Confirmation", "OrderSubmitted");
    }
}
