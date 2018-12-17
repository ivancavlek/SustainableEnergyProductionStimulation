using System;

namespace Acme.Seps.Domain.Subsidy.QueryResult
{
    public class EconometricIndexQueryResult
    {
        public DateTimeOffset Since { get; set; }
        public DateTimeOffset? Until { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
}