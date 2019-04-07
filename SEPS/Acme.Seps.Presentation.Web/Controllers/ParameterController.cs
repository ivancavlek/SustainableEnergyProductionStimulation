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
    public sealed class ParameterController : Controller
    {
        private readonly ISepsCommandHandler<CalculateNewAverageElectricEnergyProductionPriceCommand> _calculateNewAeepp;
        private readonly ISepsCommandHandler<CalculateNewConsumerPriceIndexCommand> _calculateNewCpi;
        private readonly ISepsCommandHandler<CalculateNewNaturalGasSellingPriceCommand> _calculateNewNgsp;
        private readonly ISepsCommandHandler<CorrectActiveAverageElectricEnergyProductionPriceCommand> _correctActiveAeepp;
        private readonly ISepsCommandHandler<CorrectActiveConsumerPriceIndexCommand> _correctActiveCpi;
        private readonly ISepsCommandHandler<CorrectActiveNaturalGasSellingPriceCommand> _correctActiveNgsp;
        private readonly IQueryHandler<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>> _econometricIndexesQuery;
        private readonly IQueryHandler<GetRenewableEnergySourceTariffQuery, IReadOnlyList<RenewableEnergySourceTariffQueryResult>> _renewableEnergySourceTariffsQuery;
        private readonly IQueryHandler<GetCogenerationTariffQuery, IReadOnlyList<CogenerationTariffQueryResult>> _cogenerationTariffsQuery;

        public ParameterController(
            ISepsCommandHandler<CalculateNewAverageElectricEnergyProductionPriceCommand> calculateNewAeepp,
            ISepsCommandHandler<CalculateNewConsumerPriceIndexCommand> calculateNewCpi,
            ISepsCommandHandler<CalculateNewNaturalGasSellingPriceCommand> calculateNewNgsp,
            ISepsCommandHandler<CorrectActiveAverageElectricEnergyProductionPriceCommand> correctActiveAeepp,
            ISepsCommandHandler<CorrectActiveConsumerPriceIndexCommand> correctActiveCpi,
            ISepsCommandHandler<CorrectActiveNaturalGasSellingPriceCommand> correctActiveNgsp,
            IQueryHandler<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>> econometricIndexesQuery,
            IQueryHandler<GetRenewableEnergySourceTariffQuery, IReadOnlyList<RenewableEnergySourceTariffQueryResult>> renewableEnergySourceTariffsQuery,
            IQueryHandler<GetCogenerationTariffQuery, IReadOnlyList<CogenerationTariffQueryResult>> cogenerationTariffsQuery)
        {
            _calculateNewCpi = calculateNewCpi ?? throw new ArgumentNullException(nameof(calculateNewCpi));
            _calculateNewNgsp = calculateNewNgsp ?? throw new ArgumentNullException(nameof(calculateNewNgsp));
            _calculateNewAeepp = calculateNewAeepp ?? throw new ArgumentNullException(nameof(calculateNewAeepp));
            _correctActiveAeepp = correctActiveAeepp ?? throw new ArgumentNullException(nameof(correctActiveAeepp));
            _correctActiveCpi = correctActiveCpi ?? throw new ArgumentNullException(nameof(correctActiveCpi));
            _correctActiveNgsp = correctActiveNgsp ?? throw new ArgumentNullException(nameof(correctActiveNgsp));
            _econometricIndexesQuery = econometricIndexesQuery ?? throw new ArgumentNullException(nameof(econometricIndexesQuery));
            _renewableEnergySourceTariffsQuery = renewableEnergySourceTariffsQuery ?? throw new ArgumentNullException(nameof(renewableEnergySourceTariffsQuery));
            _cogenerationTariffsQuery = cogenerationTariffsQuery ?? throw new ArgumentNullException(nameof(cogenerationTariffsQuery));
        }

        [HttpGet]
        public IActionResult GetAverageElectricEnergyProductionPrices() =>
            Ok(_econometricIndexesQuery.Handle(new GetEconometricIndexQuery
            {
                EconometricIndexType = typeof(AverageElectricEnergyProductionPrice)
            }));

        [HttpGet]
        public IActionResult GetConsumerPriceIndexes() =>
            Ok(_econometricIndexesQuery.Handle(new GetEconometricIndexQuery
            {
                EconometricIndexType = typeof(ConsumerPriceIndex)
            }));

        [HttpGet]
        public IActionResult GetNaturalGasSellingPrices() =>
            Ok(_econometricIndexesQuery.Handle(new GetEconometricIndexQuery
            {
                EconometricIndexType = typeof(NaturalGasSellingPrice)
            }));

        [HttpGet]
        public IActionResult GetRenewableEnergySourceTariffs() =>
            Ok(_renewableEnergySourceTariffsQuery.Handle(new GetRenewableEnergySourceTariffQuery()));

        [HttpGet]
        public IActionResult GetCogenerationTariffs() =>
            Ok(_cogenerationTariffsQuery.Handle(new GetCogenerationTariffQuery()));

        [HttpPost]
        public IActionResult CalculateAverageElectricEnergyProductionPrice(
            [FromBody]CalculateNewAverageElectricEnergyProductionPriceCommand calculateNewAeepp)
        {
            _calculateNewAeepp.UseCaseExecutionProcessing +=
                CalculateAverageElectricEnergyProductionPrice_UseCaseExecutionProcessing;
            _calculateNewAeepp.Handle(calculateNewAeepp);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        [HttpPost]
        public IActionResult CalculateCpi([FromBody]CalculateNewConsumerPriceIndexCommand calculateNewCpi)
        {
            _calculateNewCpi.UseCaseExecutionProcessing += CalculateCpi_UseCaseExecutionProcessing;
            _calculateNewCpi.Handle(calculateNewCpi);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        [HttpPost]
        public IActionResult CalculateNaturalGas([FromBody]CalculateNewNaturalGasSellingPriceCommand calculateNewNgsp)
        {
            _calculateNewNgsp.UseCaseExecutionProcessing += CalculateNaturalGas_UseCaseExecutionProcessing;
            _calculateNewNgsp.Handle(calculateNewNgsp);
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

        [HttpPut]
        public IActionResult CorrectActiveCpi([FromBody]CorrectActiveConsumerPriceIndexCommand correctActiveCpi)
        {
            _correctActiveCpi.UseCaseExecutionProcessing += CorrectActiveCpi_UseCaseExecutionProcessing;
            _correctActiveCpi.Handle(correctActiveCpi);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        [HttpPut] // not good, needs correction
        public IActionResult CorrectActiveNaturalGas(int id, [FromBody]CorrectActiveNaturalGasSellingPriceCommand correctActiveNgsp)
        {
            _correctActiveNgsp.UseCaseExecutionProcessing += CorrectActiveNaturalGas_UseCaseExecutionProcessing;
            _correctActiveNgsp.Handle(correctActiveNgsp);
            return Ok(); // ToDo: not in line with REST pattern, we could return latest value
        }

        private void CalculateCpi_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e)
        {
        }

        private void CalculateNaturalGas_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e)
        {
        }

        private void CalculateAverageElectricEnergyProductionPrice_UseCaseExecutionProcessing(
            object sender, EntityExecutionLoggingEventArgs e)
        {
        }

        private void CorrectActiveAverageElectricEnergyProductionPrice_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e)
        {
        }

        private void CorrectActiveCpi_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e)
        {
        }

        private void CorrectActiveNaturalGas_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e)
        {
        }
    }
}