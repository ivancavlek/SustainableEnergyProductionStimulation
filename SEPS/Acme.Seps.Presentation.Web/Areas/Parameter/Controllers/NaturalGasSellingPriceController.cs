using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Presentation.Web.DependencyInjection;
using Acme.Seps.UseCases.Subsidy.Command;
using Acme.Seps.UseCases.Subsidy.Query;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Acme.Seps.Presentation.Web.Controllers;

public sealed class NaturalGasSellingPriceController : SepsBaseController
{
    public NaturalGasSellingPriceController(ICqrsMediator mediator) : base(mediator) { }

    [HttpGet]
    public IActionResult GetNaturalGasSellingPrices() =>
        Ok(_mediator.Handle<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>>(
            new GetEconometricIndexQuery { EconometricIndexType = typeof(NaturalGasSellingPrice) }));

    [HttpPost]
    public IActionResult CalculateNaturalGas([FromBody]CalculateNewNaturalGasSellingPriceCommand calculateNewNgsp)
    {
        _mediator.Send(calculateNewNgsp);
        return Ok(); // ToDo: not in line with REST pattern, we could return latest value
    }

    [HttpPut] // not good, needs correction
    public IActionResult CorrectActiveNaturalGas(int id, [FromBody]CorrectActiveNaturalGasSellingPriceCommand correctActiveNgsp)
    {
        _mediator.Send(correctActiveNgsp);
        return Ok(); // ToDo: not in line with REST pattern, we could return latest value
    }
}