namespace Acme.Seps.Domain.Subsidy.QueryResult
{
    public class ActiveTariffsQueryResult
    {
        public string ValidFrom { get; set; }
        public string ValidTill { get; set; }
        public int LowerProductionLimit { get; set; }
        public int UpperProductionLimit { get; set; }
        public decimal LowerRate { get; set; }
        public decimal HigherRate { get; set; }
        public decimal EconometricIndexAmount { get; set; }
    }
}