using Acme.Domain.Base.Query;

namespace Acme.Domain.Base.QueryHandler;

/// <summary>
/// <see href="https://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=92">Query credits</see>
/// </summary>
public interface IQueryHandler<TQuery, TQueryResult> where TQuery : IQuery<TQueryResult>
{
    TQueryResult Handle(TQuery query);
}