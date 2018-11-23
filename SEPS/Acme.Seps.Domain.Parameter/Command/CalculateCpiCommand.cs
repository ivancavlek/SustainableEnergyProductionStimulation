using Acme.Domain.Base.Command;

namespace Acme.Seps.Domain.Parameter.Command
{
    public sealed class CalculateCpiCommand : IBaseCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
}