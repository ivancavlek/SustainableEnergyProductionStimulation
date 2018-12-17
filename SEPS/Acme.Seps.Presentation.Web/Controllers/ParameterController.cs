using Acme.Domain.Base.QueryHandler;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Subsidy.Command;
using Acme.Seps.Domain.Subsidy.Command.Entity;
using Acme.Seps.Domain.Subsidy.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Presentation.Web.Controllers
{
    [Route("api/[controller]")]
    public sealed class ParameterController : Controller
    {
        private readonly ISepsCommandHandler<CalculateConsumerPriceIndexCommand> _calculateCpi;
        private readonly ISepsCommandHandler<CalculateNaturalGasSellingPriceCommand> _calculateNaturalGas;
        private readonly ISepsCommandHandler<CorrectActiveConsumerPriceIndexCommand> _correctActiveCpi;
        private readonly ISepsCommandHandler<CorrectActiveNaturalGasSellingPriceCommand> _correctActiveNaturalGas;
        private readonly IQueryHandler<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>> _econometricIndexesQuery;
        private readonly IQueryHandler<GetRenewableEnergySourceTariffQuery, IReadOnlyList<RenewableEnergySourceTariffQueryResult>> _renewableEnergySourceTariffsQuery;
        private readonly IQueryHandler<GetCogenerationTariffQuery, IReadOnlyList<CogenerationTariffQueryResult>> _cogenerationTariffsQuery;

        public ParameterController(
            ISepsCommandHandler<CalculateConsumerPriceIndexCommand> calculateCpi,
            ISepsCommandHandler<CalculateNaturalGasSellingPriceCommand> calculateNaturalGas,
            ISepsCommandHandler<CorrectActiveConsumerPriceIndexCommand> correctActiveCpi,
            ISepsCommandHandler<CorrectActiveNaturalGasSellingPriceCommand> correctActiveNaturalGas,
            IQueryHandler<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>> econometricIndexesQuery,
            IQueryHandler<GetRenewableEnergySourceTariffQuery, IReadOnlyList<RenewableEnergySourceTariffQueryResult>> renewableEnergySourceTariffsQuery,
            IQueryHandler<GetCogenerationTariffQuery, IReadOnlyList<CogenerationTariffQueryResult>> cogenerationTariffsQuery)
        {
            _calculateCpi = calculateCpi ?? throw new ArgumentNullException(nameof(calculateCpi));
            _calculateNaturalGas = calculateNaturalGas ?? throw new ArgumentNullException(nameof(calculateNaturalGas));
            _correctActiveCpi = correctActiveCpi ?? throw new ArgumentNullException(nameof(correctActiveCpi));
            _correctActiveNaturalGas = correctActiveNaturalGas ?? throw new ArgumentNullException(nameof(correctActiveNaturalGas));
            _econometricIndexesQuery = econometricIndexesQuery ?? throw new ArgumentNullException(nameof(econometricIndexesQuery));
            _renewableEnergySourceTariffsQuery = renewableEnergySourceTariffsQuery ?? throw new ArgumentNullException(nameof(renewableEnergySourceTariffsQuery));
            _cogenerationTariffsQuery = cogenerationTariffsQuery ?? throw new ArgumentNullException(nameof(cogenerationTariffsQuery));
        }

        [HttpGet]
        [Route("GetConsumerPriceIndexes")]
        public IActionResult GetConsumerPriceIndexes() =>
            Ok(_econometricIndexesQuery.Handle(new GetEconometricIndexQuery
            {
                EconometricIndexType = typeof(ConsumerPriceIndex)
            }));

        [HttpGet]
        [Route("GetNaturalGasSellingPrices")]
        public IActionResult GetNaturalGasSellingPrices() =>
            Ok(_econometricIndexesQuery.Handle(new GetEconometricIndexQuery
            {
                EconometricIndexType = typeof(NaturalGasSellingPrice)
            }));

        [HttpGet]
        [Route("GetRenewableEnergySourceTariffs")]
        public IActionResult GetRenewableEnergySourceTariffs() =>
            Ok(_renewableEnergySourceTariffsQuery.Handle(new GetRenewableEnergySourceTariffQuery()));

        [HttpGet]
        [Route("GetCogenerationTariffs")]
        public IActionResult GetCogenerationTariffs() =>
            Ok(_cogenerationTariffsQuery.Handle(new GetCogenerationTariffQuery()));

        // GET api/<controller>/5
        // questionable do we need this
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost("CalculateCpi")]
        public IActionResult CalculateCpi([FromBody]CalculateConsumerPriceIndexCommand calculateCpiCommand)
        {
            _calculateCpi.UseCaseExecutionProcessing += CalculateCpi_UseCaseExecutionProcessing;
            _calculateCpi.Handle(calculateCpiCommand);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        [HttpPost]
        [Route("CalculateNaturalGas")]
        public IActionResult CalculateNaturalGas([FromBody]CalculateNaturalGasSellingPriceCommand calculateNaturalGas)
        {
            _calculateNaturalGas.UseCaseExecutionProcessing += CalculateNaturalGas_UseCaseExecutionProcessing;
            _calculateNaturalGas.Handle(calculateNaturalGas);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        [HttpPut]
        [Route("CorrectActiveCpi")]
        public IActionResult CorrectActiveCpi([FromBody]CorrectActiveConsumerPriceIndexCommand correctActiveCpi)
        {
            _correctActiveCpi.UseCaseExecutionProcessing += CorrectActiveCpi_UseCaseExecutionProcessing;
            _correctActiveCpi.Handle(correctActiveCpi);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        [HttpPut("{id}")] // not good, needs correction
        [Route("CorrectActiveNaturalGas")]
        public void CorrectActiveNaturalGas(int id, [FromBody]CorrectActiveNaturalGasSellingPriceCommand correctActiveNaturalGas)
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