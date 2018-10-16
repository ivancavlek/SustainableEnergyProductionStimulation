using System.Collections.Generic;
using Acme.Domain.Base.Command;
using Acme.Seps.Domain.Parameter.Entity;

namespace Acme.Seps.Domain.Parameter.Command
{
    public sealed class CorrectActiveCpiCommand : BaseCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public ConsumerPriceIndex ActiveCpi { get; set; }
        public IEnumerable<RenewableEnergySourceTariff> ActiveResTariffs { get; set; }
    }
}