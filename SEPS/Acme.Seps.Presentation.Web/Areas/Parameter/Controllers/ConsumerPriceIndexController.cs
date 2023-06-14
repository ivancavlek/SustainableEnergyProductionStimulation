using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Presentation.Web.DependencyInjection;
using Acme.Seps.UseCases.Subsidy.Command;
using Acme.Seps.UseCases.Subsidy.Query;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Acme.Seps.Presentation.Web.Controllers;

public sealed class ConsumerPriceIndexController : SepsBaseController
{
    public ConsumerPriceIndexController(ICqrsMediator mediator) : base(mediator) { }

    [HttpGet]
    public IActionResult GetConsumerPriceIndexes() =>
         Ok(_mediator.Handle<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>>(
            new GetEconometricIndexQuery { EconometricIndexType = typeof(ConsumerPriceIndex) }));

    [HttpPost]
    public IActionResult CalculateCpi([FromBody]CalculateNewConsumerPriceIndexCommand calculateNewCpi)
    {
        _mediator.Send(calculateNewCpi);
        return Ok(); // ToDo: not in line with REST pattern, we could return latest value
    }

    [HttpPut]
    public IActionResult CorrectActiveCpi([FromBody]CorrectActiveConsumerPriceIndexCommand correctActiveCpi)
    {
        _mediator.Send(correctActiveCpi);
        return Ok(); // ToDo: not in line with REST pattern, we could return latest value
    }
}