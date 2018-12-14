using System;

namespace Acme.Seps.Domain.Subsidy.QueryResult
{
    public class EconometricIndexQueryResult
    {
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset? ActiveTill { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
}