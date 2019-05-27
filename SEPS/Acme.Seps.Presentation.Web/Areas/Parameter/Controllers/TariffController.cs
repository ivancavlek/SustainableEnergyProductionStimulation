using Acme.Domain.Base.QueryHandler;
using Acme.Seps.UseCases.Subsidy.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Presentation.Web.Controllers
{
    public sealed class TariffController : Controller
    {
        private readonly IQueryHandler<GetRenewableEnergySourceTariffQuery, IReadOnlyList<RenewableEnergySourceTariffQueryResult>> _renewableEnergySourceTariffsQuery;
        private readonly IQueryHandler<GetCogenerationTariffQuery, IReadOnlyList<CogenerationTariffQueryResult>> _cogenerationTariffsQuery;

        public TariffController(
            IQueryHandler<GetRenewableEnergySourceTariffQuery, IReadOnlyList<RenewableEnergySourceTariffQueryResult>> renewableEnergySourceTariffsQuery,
            IQueryHandler<GetCogenerationTariffQuery, IReadOnlyList<CogenerationTariffQueryResult>> cogenerationTariffsQuery)
        {
            _renewableEnergySourceTariffsQuery = renewableEnergySourceTariffsQuery ?? throw new ArgumentNullException(nameof(renewableEnergySourceTariffsQuery));
            _cogenerationTariffsQuery = cogenerationTariffsQuery ?? throw new ArgumentNullException(nameof(cogenerationTariffsQuery));
        }

        [HttpGet]
        public IActionResult GetRenewableEnergySourceTariffs() =>
            Ok(_renewableEnergySourceTariffsQuery.Handle(new GetRenewableEnergySourceTariffQuery()));

        [HttpGet]
        public IActionResult GetCogenerationTariffs() =>
            Ok(_cogenerationTariffsQuery.Handle(new GetCogenerationTariffQuery()));
    }
}