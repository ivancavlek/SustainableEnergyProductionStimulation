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
    public sealed class NaturalGasSellingPriceController : Controller
    {
        private readonly IQueryHandler<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>> _econometricIndexesQuery;
        private readonly ISepsCommandHandler<CalculateNewNaturalGasSellingPriceCommand> _calculateNewNgsp;
        private readonly ISepsCommandHandler<CorrectActiveNaturalGasSellingPriceCommand> _correctActiveNgsp;

        public NaturalGasSellingPriceController(
            IQueryHandler<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>> econometricIndexesQuery,
            ISepsCommandHandler<CalculateNewNaturalGasSellingPriceCommand> calculateNewNgsp,
            ISepsCommandHandler<CorrectActiveNaturalGasSellingPriceCommand> correctActiveNgsp)
        {
            _econometricIndexesQuery = econometricIndexesQuery ?? throw new ArgumentNullException(nameof(econometricIndexesQuery));
            _calculateNewNgsp = calculateNewNgsp ?? throw new ArgumentNullException(nameof(calculateNewNgsp));
            _correctActiveNgsp = correctActiveNgsp ?? throw new ArgumentNullException(nameof(correctActiveNgsp));
        }

        [HttpGet]
        public IActionResult GetNaturalGasSellingPrices() =>
            Ok(_econometricIndexesQuery.Handle(new GetEconometricIndexQuery
            {
                EconometricIndexType = typeof(NaturalGasSellingPrice)
            }));

        [HttpPost]
        public IActionResult CalculateNaturalGas([FromBody]CalculateNewNaturalGasSellingPriceCommand calculateNewNgsp)
        {
            _calculateNewNgsp.UseCaseExecutionProcessing += CalculateNaturalGas_UseCaseExecutionProcessing;
            _calculateNewNgsp.Handle(calculateNewNgsp);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        [HttpPut] // not good, needs correction
        public IActionResult CorrectActiveNaturalGas(int id, [FromBody]CorrectActiveNaturalGasSellingPriceCommand correctActiveNgsp)
        {
            _correctActiveNgsp.UseCaseExecutionProcessing += CorrectActiveNaturalGas_UseCaseExecutionProcessing;
            _correctActiveNgsp.Handle(correctActiveNgsp);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        private void CalculateNaturalGas_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e)
        {
        }

        private void CorrectActiveNaturalGas_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e)
        {
        }
    }
}