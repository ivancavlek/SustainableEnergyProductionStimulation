using Acme.Seps.Presentation.Web.DependencyInjection;
using Acme.Seps.UseCases.Subsidy.Query;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Acme.Seps.Presentation.Web.Controllers;

public sealed class TariffController : SepsBaseController
{
    public TariffController(ICqrsMediator mediator) : base(mediator) { }

    [HttpGet]
    public IActionResult GetRenewableEnergySourceTariffs() =>
        Ok(_mediator.Handle<GetRenewableEnergySourceTariffQuery, IReadOnlyList<RenewableEnergySourceTariffQueryResult>>(
            new GetRenewableEnergySourceTariffQuery()));

    [HttpGet]
    public IActionResult GetCogenerationTariffs() =>
        Ok(_mediator.Handle<GetCogenerationTariffQuery, IReadOnlyList<CogenerationTariffQueryResult>>(
            new GetCogenerationTariffQuery()));
}