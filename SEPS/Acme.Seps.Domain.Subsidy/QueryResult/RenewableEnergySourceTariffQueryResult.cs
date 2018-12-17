using System;

namespace Acme.Seps.Domain.Subsidy.QueryResult
{
    public class RenewableEnergySourceTariffQueryResult
    {
        public string ContractLabel { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTimeOffset Since { get; set; }
        public DateTimeOffset? Until { get; set; }
        public int? LowerProductionLimit { get; set; }
        public int? UpperProductionLimit { get; set; }
        public decimal LowerRate { get; set; }
        public decimal HigherRate { get; set; }
        public decimal ConsumerPriceIndexAmount { get; set; }
        public bool ConsumesFuel { get; set; }
    }
}