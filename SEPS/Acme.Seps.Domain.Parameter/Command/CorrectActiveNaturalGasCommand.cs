using System.Collections.Generic;
using Acme.Domain.Base.Command;
using Acme.Seps.Domain.Parameter.Entity;

namespace Acme.Seps.Domain.Parameter.Command
{
    public sealed class CorrectActiveNaturalGasCommand : BaseCommand
    {
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public NaturalGasSellingPrice ActiveNaturalGasSellingPrice { get; set; }
        public IEnumerable<CogenerationTariff> ActiveCogenerationTariffs { get; set; }
        public IEnumerable<NaturalGasSellingPrice> YearsNaturalGasSellingPrices { get; set; }
    }
}