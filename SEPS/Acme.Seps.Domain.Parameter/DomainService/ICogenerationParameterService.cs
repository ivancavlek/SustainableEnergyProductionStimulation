namespace Acme.Seps.Domain.Parameter.DomainService
{
    public interface ICogenerationParameterService
    {
        decimal GetFrom(decimal yaepAmount, decimal naturalGasSellingPriceAmount);
    }
}