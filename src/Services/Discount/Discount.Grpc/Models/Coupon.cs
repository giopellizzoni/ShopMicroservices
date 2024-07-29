namespace Discount.Grpc.Models;

public class Coupon
{
    public Coupon()
    {
    }

    public Coupon(string productName, int amount, string description)
    {
        ProductName = productName;
        Amount = amount;
        Description = description;
    }

    public int Id { get; set; }
    public string ProductName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int Amount { get; set; }
}
