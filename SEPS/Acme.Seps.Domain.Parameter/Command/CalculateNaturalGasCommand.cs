using Acme.Domain.Base.Command;

namespace Acme.Seps.Domain.Parameter.Command
{
    public class CalculateNaturalGasCommand : BaseCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}