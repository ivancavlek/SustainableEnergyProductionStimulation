namespace Acme.Domain.Base.Factory
{
    public interface IMapperFactory
    {
        TDestination Map<TSource, TDestination>(TSource source);

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }
}