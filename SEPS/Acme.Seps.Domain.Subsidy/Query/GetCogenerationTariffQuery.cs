using Acme.Domain.Base.Query;
using Acme.Seps.Domain.Subsidy.QueryResult;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Subsidy.Query
{
    public class GetCogenerationTariffQuery : IQuery<IReadOnlyList<CogenerationTariffQueryResult>>
    {
    }
}