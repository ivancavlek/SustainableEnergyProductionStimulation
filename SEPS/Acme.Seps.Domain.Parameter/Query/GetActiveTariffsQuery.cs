using Acme.Domain.Base.Query;
using Acme.Seps.Domain.Parameter.QueryResult;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Parameter.Query
{
    public class GetActiveTariffsQuery : IQuery<IReadOnlyList<ActiveTariffsQueryResult>>
    {
        public Type TariffType { get; set; }
    }
}