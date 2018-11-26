namespace Acme.Seps.Domain.Subsidy.Command
{
    public sealed class CorrectActiveNaturalGasCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}