using Acme.Domain.Base.Query;

namespace Acme.Domain.Base.QueryHandler
{
    public interface IQueryHandler<TQuery, TQueryResult> where TQuery : IQuery<TQueryResult>
    {
        TQueryResult Handle(TQuery query);
    }
}