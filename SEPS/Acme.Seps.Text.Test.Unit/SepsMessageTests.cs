using FluentAssertions;
using System;
using System.Globalization;

namespace Acme.Seps.Text.Test.Unit
{
    public class SepsMessageTests
    {
        public SepsMessageTests()
        {
            var cultureInfo = new CultureInfo("hr-HR");

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }

        public void CannotDeactivateInactiveEntityIsCorrect() =>
            SepsMessage.CannotDeactivateInactiveEntity("TestEntity")
                .Should().Be("Cannot deactivate inactive Test entity");

        public void EntityNotSetIsCorrect() =>
            SepsMessage.EntityNotSet("TestEntity")
                .Should().Be("Test entity must be set");

        public void InactiveExceptionIsCorrect() =>
            SepsMessage.InactiveException("TestEntity")
                .Should().Be("Test entity must be active");

        public void InitialValuesMustNotBeChangedIsCorrect() =>
            SepsMessage.InitialValuesMustNotBeChanged()
                .Should().Be("Initial values must not be changed");

        public void InsertParameterIsCorrect() =>
            SepsMessage.InsertParameter("TestEntity", new DateTime(2010, 4, 1), new DateTime(2010, 5, 1), 20M)
                .Should().Be("Created test entity for period 01.04.2010. - 01.05.2010. with amount 20,00 kn");

        public void InsertTariffIsCorrect() =>
            SepsMessage.InsertTariff("TestEntity", new DateTime(2010, 4, 1), new DateTime(2010, 5, 1), 20M, 50M)
                .Should().Be("Created test entity for period 01.04.2010. - 01.05.2010. with amounts 20,00 kn/50,00 kn");

        public void ParameterCorrectionIsCorrect() =>
            SepsMessage.ParameterCorrection("TestEntity", new DateTime(2010, 4, 1), new DateTime(2010, 5, 1), 20M)
                .Should().Be("Corrected test entity for period 01.04.2010. - 01.05.2010. with amount 20,00 kn");

        public void SuccessfulSaveIsCorrect() =>
            SepsMessage.SuccessfulSave()
                .Should().Be("Successful save of entities to database");

        public void TariffCorrectionIsCorrect() =>
            SepsMessage.TariffCorrection("TestEntity", new DateTime(2010, 4, 1), new DateTime(2010, 5, 1), 20M, 50M)
                .Should().Be("Corrected test entity for period 01.04.2010. - 01.05.2010. with amounts 20,00 kn/50,00 kn");

        public void ValueHigherThanTheOtherIsCorrect() =>
            SepsMessage.ValueHigherThanTheOther("HigherAmount", "LowerAmount")
                .Should().Be("Higher amount must be greater than lower amount");

        public void ValueZeroOrAboveIsCorrect() =>
            SepsMessage.ValueZeroOrAbove("TestEntity")
                .Should().Be("Test entity must be zero or above");
    }
}