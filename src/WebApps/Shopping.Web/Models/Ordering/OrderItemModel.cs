namespace Shopping.Web.Models.Ordering;

public record OrderItemModel(
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    decimal Price);