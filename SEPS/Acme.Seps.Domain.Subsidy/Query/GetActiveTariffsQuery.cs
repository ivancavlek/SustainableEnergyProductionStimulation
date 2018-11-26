using Acme.Domain.Base.Query;
using Acme.Seps.Domain.Subsidy.QueryResult;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Subsidy.Query
{
    public class GetActiveTariffsQuery : IQuery<IReadOnlyList<ActiveTariffsQueryResult>>
    {
        public Type TariffType { get; set; }
    }
}