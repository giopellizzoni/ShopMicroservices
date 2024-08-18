using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Shopping.Web.Pages;


public class ProductListModel(
    ICatalogService catalogService,
    IBasketService basketService,
    ILogger<ProductListModel> logger) : PageModel
{
    public IEnumerable<string> CategoryList { get; set; } = [];
    public IEnumerable<ProductModel> ProductList { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string SelectedCategory { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string categoryName)
    {
        await LogTokenAndClaims();
        var response = await catalogService.GetProducts();

        CategoryList = response.Products.SelectMany(p => p.Categories).Distinct();

        if (!string.IsNullOrEmpty(categoryName))
        {
            ProductList = response.Products.Where(p => p.Categories.Contains(categoryName)).ToList();
            SelectedCategory = categoryName;
        }
        else
        {
            ProductList = response.Products;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAddToCartAsync(Guid productId)
    {
        logger.LogInformation("Add to cart button clicked");
        var productResponse = await catalogService.GetProductsById(productId);

        var basket = await basketService.LoadUserBasket();

        basket.Items.Add(new ShoppingCartItemModel
        {
            ProductId = productId,
            ProductName = productResponse.Product.Name,
            Price = productResponse.Product.Price,
            Quantity = 1,
            Color = "Black"
        });
        await basketService.StoreBasket(new StoreBasketRequest(basket));
        return RedirectToPage("Cart");
    }
    
    private async Task LogTokenAndClaims()
    {
        var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
        Debug.WriteLine($"Identity Token: {identityToken}");

        foreach (var claim in User.Claims)
        {
            Debug.WriteLine($"Claim type: {claim.Type}");
        }
    }
}
