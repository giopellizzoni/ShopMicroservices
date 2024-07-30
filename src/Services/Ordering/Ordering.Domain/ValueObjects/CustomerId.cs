namespace Ordering.Domain.ValueObjects;
public record CustomerId
{
    public Guid Value { get; }

    private CustomerId(Guid value) => Value = value;

    protected CustomerId()
    {
    }

    public static CustomerId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("CustomerId cannot be empty");
        }

        return new CustomerId(value);
    }
}
