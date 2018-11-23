using Acme.Domain.Base.Command;

namespace Acme.Seps.Domain.Parameter.Command
{
    public sealed class CalculateNaturalGasCommand : IBaseCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}