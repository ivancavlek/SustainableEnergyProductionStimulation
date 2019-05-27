using Acme.Domain.Base.QueryHandler;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.UseCases.Subsidy.Command;
using Acme.Seps.UseCases.Subsidy.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Presentation.Web.Controllers
{
    public sealed class AverageElectricEnergyProductionPriceController : Controller
    {
        private readonly IQueryHandler<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>> _econometricIndexesQuery;
        private readonly ISepsCommandHandler<CalculateNewAverageElectricEnergyProductionPriceCommand> _calculateNewAeepp;
        private readonly ISepsCommandHandler<CorrectActiveAverageElectricEnergyProductionPriceCommand> _correctActiveAeepp;

        public AverageElectricEnergyProductionPriceController(
            IQueryHandler<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>> econometricIndexesQuery,
            ISepsCommandHandler<CalculateNewAverageElectricEnergyProductionPriceCommand> calculateNewAeepp,
            ISepsCommandHandler<CorrectActiveAverageElectricEnergyProductionPriceCommand> correctActiveAeepp)
        {
            _econometricIndexesQuery = econometricIndexesQuery ?? throw new ArgumentNullException(nameof(econometricIndexesQuery));
            _calculateNewAeepp = calculateNewAeepp ?? throw new ArgumentNullException(nameof(calculateNewAeepp));
            _correctActiveAeepp = correctActiveAeepp ?? throw new ArgumentNullException(nameof(correctActiveAeepp));
        }

        [HttpGet]
        public IActionResult GetAverageElectricEnergyProductionPrices() =>
            Ok(_econometricIndexesQuery.Handle(new GetEconometricIndexQuery
            {
                EconometricIndexType = typeof(AverageElectricEnergyProductionPrice)
            }));

        [HttpPost]
        public IActionResult CalculateAverageElectricEnergyProductionPrice(
            [FromBody]CalculateNewAverageElectricEnergyProductionPriceCommand calculateNewAeepp)
        {
            _calculateNewAeepp.UseCaseExecutionProcessing +=
                CalculateAverageElectricEnergyProductionPrice_UseCaseExecutionProcessing;
            _calculateNewAeepp.Handle(calculateNewAeepp);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        [HttpPut] // not good, needs correction
        public IActionResult CorrectActiveAverageElectricEnergyProductionPrice(
            int id, [FromBody]CorrectActiveAverageElectricEnergyProductionPriceCommand correctActiveAeepp)
        {
            _correctActiveAeepp.UseCaseExecutionProcessing +=
                CorrectActiveAverageElectricEnergyProductionPrice_UseCaseExecutionProcessing;
            _correctActiveAeepp.Handle(correctActiveAeepp);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        private void CalculateAverageElectricEnergyProductionPrice_UseCaseExecutionProcessing(
            object sender, EntityExecutionLoggingEventArgs e)
        {
        }

        private void CorrectActiveAverageElectricEnergyProductionPrice_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e)
        {
        }
    }
}