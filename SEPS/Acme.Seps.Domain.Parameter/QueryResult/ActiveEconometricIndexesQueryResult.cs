namespace Acme.Seps.Domain.Parameter.QueryResult
{
    public class ActiveEconometricIndexesQueryResult
    {
        public string ValidFrom { get; set; }
        public string ValidTill { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
}