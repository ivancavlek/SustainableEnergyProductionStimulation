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
    public sealed class ConsumerPriceIndexController : Controller
    {
        private readonly IQueryHandler<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>> _econometricIndexesQuery;
        private readonly ISepsCommandHandler<CalculateNewConsumerPriceIndexCommand> _calculateNewCpi;
        private readonly ISepsCommandHandler<CorrectActiveConsumerPriceIndexCommand> _correctActiveCpi;

        public ConsumerPriceIndexController(
            IQueryHandler<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>> econometricIndexesQuery,
            ISepsCommandHandler<CalculateNewConsumerPriceIndexCommand> calculateNewCpi,
            ISepsCommandHandler<CorrectActiveConsumerPriceIndexCommand> correctActiveCpi)
        {
            _calculateNewCpi = calculateNewCpi ?? throw new ArgumentNullException(nameof(calculateNewCpi));
            _correctActiveCpi = correctActiveCpi ?? throw new ArgumentNullException(nameof(correctActiveCpi));
            _econometricIndexesQuery = econometricIndexesQuery ?? throw new ArgumentNullException(nameof(econometricIndexesQuery));
        }

        [HttpGet]
        public IActionResult GetConsumerPriceIndexes() =>
            Ok(_econometricIndexesQuery.Handle(new GetEconometricIndexQuery
            {
                EconometricIndexType = typeof(ConsumerPriceIndex)
            }));

        [HttpPost]
        public IActionResult CalculateCpi([FromBody]CalculateNewConsumerPriceIndexCommand calculateNewCpi)
        {
            _calculateNewCpi.UseCaseExecutionProcessing += CalculateCpi_UseCaseExecutionProcessing;
            _calculateNewCpi.Handle(calculateNewCpi);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        [HttpPut]
        public IActionResult CorrectActiveCpi([FromBody]CorrectActiveConsumerPriceIndexCommand correctActiveCpi)
        {
            _correctActiveCpi.UseCaseExecutionProcessing += CorrectActiveCpi_UseCaseExecutionProcessing;
            _correctActiveCpi.Handle(correctActiveCpi);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        private void CalculateCpi_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e)
        {
        }

        private void CorrectActiveCpi_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e)
        {
        }
    }
}