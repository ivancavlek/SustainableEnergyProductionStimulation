namespace Acme.Domain.Base.DomainService
{
    public interface IMapper
    {
        TDestination Map<TSource, TDestination>(TSource source);

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }
}