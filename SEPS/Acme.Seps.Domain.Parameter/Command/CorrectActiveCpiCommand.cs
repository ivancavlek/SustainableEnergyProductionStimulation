using Acme.Domain.Base.Command;
using Acme.Seps.Domain.Parameter.Entity;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Parameter.Command
{
    public class CorrectActiveCpiCommand : BaseCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public ConsumerPriceIndex ActiveCpi { get; set; }
        public IEnumerable<RenewableEnergySourceTariff> ActiveResTariffs { get; set; }
    }
}