using System;

namespace Acme.Seps.Domain.Subsidy.QueryResult
{
    public class CogenerationTariffQueryResult
    {
        public string ContractLabel { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset? ActiveTill { get; set; }
        public int? LowerProductionLimit { get; set; }
        public int? UpperProductionLimit { get; set; }
        public decimal LowerRate { get; set; }
        public decimal HigherRate { get; set; }
        public decimal NaturalGasSellingPriceAmount { get; set; }
        public decimal MonthlyAverageElectricEnergyProductionPriceAmount { get; set; }
        public bool ConsumesFuel { get; set; }
    }
}