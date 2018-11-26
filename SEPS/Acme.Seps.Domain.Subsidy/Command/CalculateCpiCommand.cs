namespace Acme.Seps.Domain.Subsidy.Command
{
    public sealed class CalculateCpiCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
}