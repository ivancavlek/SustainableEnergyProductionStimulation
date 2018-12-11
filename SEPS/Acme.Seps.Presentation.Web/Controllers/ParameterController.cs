using Acme.Domain.Base.QueryHandler;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Subsidy.Command;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Domain.Subsidy.Query;
using Acme.Seps.Domain.Subsidy.QueryResult;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Presentation.Web.Controllers
{
    [Route("api/[controller]")]
    public sealed class ParameterController : Controller
    {
        private readonly ISepsCommandHandler<CalculateCpiCommand> _calculateCpi;
        private readonly ISepsCommandHandler<CalculateNaturalGasCommand> _calculateNaturalGas;
        private readonly ISepsCommandHandler<CorrectActiveCpiCommand> _correctActiveCpi;
        private readonly ISepsCommandHandler<CorrectActiveNaturalGasCommand> _correctActiveNaturalGas;
        private readonly IQueryHandler<GetActiveEconometricIndexesQuery, IReadOnlyList<ActiveEconometricIndexesQueryResult>> _activeEconometricIndexesQuery;
        private readonly IQueryHandler<GetActiveTariffsQuery, IReadOnlyList<ActiveTariffsQueryResult>> _activeTariffsQuery;

        public ParameterController(
            ISepsCommandHandler<CalculateCpiCommand> calculateCpi,
            ISepsCommandHandler<CalculateNaturalGasCommand> calculateNaturalGas,
            ISepsCommandHandler<CorrectActiveCpiCommand> correctActiveCpi,
            ISepsCommandHandler<CorrectActiveNaturalGasCommand> correctActiveNaturalGas,
            IQueryHandler<GetActiveEconometricIndexesQuery, IReadOnlyList<ActiveEconometricIndexesQueryResult>> activeEconometricIndexesQuery,
            IQueryHandler<GetActiveTariffsQuery, IReadOnlyList<ActiveTariffsQueryResult>> activeTariffsQuery)
        {
            _calculateCpi = calculateCpi ?? throw new ArgumentNullException(nameof(calculateCpi));
            _calculateNaturalGas = calculateNaturalGas ?? throw new ArgumentNullException(nameof(calculateNaturalGas));
            _correctActiveCpi = correctActiveCpi ?? throw new ArgumentNullException(nameof(correctActiveCpi));
            _correctActiveNaturalGas = correctActiveNaturalGas ?? throw new ArgumentNullException(nameof(correctActiveNaturalGas));
            _activeEconometricIndexesQuery = activeEconometricIndexesQuery ?? throw new ArgumentNullException(nameof(activeEconometricIndexesQuery));
            _activeTariffsQuery = activeTariffsQuery ?? throw new ArgumentNullException(nameof(activeTariffsQuery));
        }

        [HttpGet]
        [Route("GetActiveConsumerPriceIndexes")]
        public IActionResult GetActiveConsumerPriceIndexes() =>
            Ok(_activeEconometricIndexesQuery.Handle(new GetActiveEconometricIndexesQuery
            {
                EconometricIndexType = typeof(ConsumerPriceIndex)
            }));

        [HttpGet]
        [Route("GetActiveNaturalGasSellingPrices")]
        public IActionResult GetActiveNaturalGasSellingPrices() =>
            Ok(_activeEconometricIndexesQuery.Handle(new GetActiveEconometricIndexesQuery
            {
                EconometricIndexType = typeof(NaturalGasSellingPrice)
            }));

        [HttpGet]
        [Route("GetActiveRenewableEnergySourceTariffs")]
        public IActionResult GetActiveRenewableEnergySourceTariffs() =>
            Ok(_activeTariffsQuery.Handle(new GetActiveTariffsQuery
            {
                TariffType = typeof(RenewableEnergySourceTariff)
            }));

        [HttpGet]
        [Route("GetActiveCogenerationTariffs")]
        public IActionResult GetActiveCogenerationTariffs() =>
            Ok(_activeTariffsQuery.Handle(new GetActiveTariffsQuery
            {
                TariffType = typeof(CogenerationTariff)
            }));

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost("CalculateCpi")]
        public IActionResult CalculateCpi([FromBody]CalculateCpiCommand calculateCpiCommand)
        {
            _calculateCpi.UseCaseExecutionProcessing += CalculateCpi_UseCaseExecutionProcessing;
            _calculateCpi.Handle(calculateCpiCommand);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        [HttpPost]
        [Route("CalculateNaturalGas")]
        public IActionResult CalculateNaturalGas([FromBody]CalculateNaturalGasCommand calculateNaturalGas)
        {
            _calculateNaturalGas.UseCaseExecutionProcessing += CalculateNaturalGas_UseCaseExecutionProcessing;
            _calculateNaturalGas.Handle(calculateNaturalGas);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        [HttpPut]
        [Route("CorrectActiveCpi")]
        public IActionResult CorrectActiveCpi([FromBody]CorrectActiveCpiCommand correctActiveCpi)
        {
            _correctActiveCpi.UseCaseExecutionProcessing += CorrectActiveCpi_UseCaseExecutionProcessing;
            _correctActiveCpi.Handle(correctActiveCpi);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        [HttpPut("{id}")] // not good, needs correction
        [Route("CorrectActiveNaturalGas")]
        public void CorrectActiveNaturalGas(int id, [FromBody]CorrectActiveNaturalGasCommand correctActiveNaturalGas)
        {
            _correctActiveNaturalGas.Handle(correctActiveNaturalGas);
        }

        private void CalculateCpi_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e)
        {
        }

        private void CalculateNaturalGas_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e)
        {
        }

        private void CorrectActiveCpi_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e)
        {
        }
    }
}