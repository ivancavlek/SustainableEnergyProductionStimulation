namespace Acme.Seps.Domain.Parameter.Command
{
    public sealed class CalculateCpiCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
}