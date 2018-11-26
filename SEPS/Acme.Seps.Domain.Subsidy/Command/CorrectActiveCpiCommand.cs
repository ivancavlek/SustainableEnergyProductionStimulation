namespace Acme.Seps.Domain.Subsidy.Command
{
    public sealed class CorrectActiveCpiCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
}