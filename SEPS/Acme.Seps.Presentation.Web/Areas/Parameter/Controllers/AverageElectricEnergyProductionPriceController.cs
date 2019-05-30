using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Presentation.Web.DependencyInjection;
using Acme.Seps.UseCases.Subsidy.Command;
using Acme.Seps.UseCases.Subsidy.Query;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Acme.Seps.Presentation.Web.Controllers
{
    public sealed class AverageElectricEnergyProductionPriceController : SepsBaseController
    {
        public AverageElectricEnergyProductionPriceController(ICqrsMediator mediator) : base(mediator) { }

        [HttpGet]
        public IActionResult GetAverageElectricEnergyProductionPrices() =>
            Ok(_mediator.Handle<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>>(
                new GetEconometricIndexQuery { EconometricIndexType = typeof(AverageElectricEnergyProductionPrice) }));

        [HttpPost]
        public IActionResult CalculateAverageElectricEnergyProductionPrice(
            [FromBody]CalculateNewAverageElectricEnergyProductionPriceCommand calculateNewAeepp)
        {
            _mediator.Send(calculateNewAeepp);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        [HttpPut] // not good, needs correction
        public IActionResult CorrectActiveAverageElectricEnergyProductionPrice(
            int id, [FromBody]CorrectActiveAverageElectricEnergyProductionPriceCommand correctActiveAeepp)
        {
            _mediator.Send(correctActiveAeepp);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }
    }
}