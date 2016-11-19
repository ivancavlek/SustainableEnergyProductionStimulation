namespace Acme.Seps.Domain.Parameter.Test.Unit.Factory
{
    //public class YearlyAverageElectricEnergyProductionPriceFactoryTests
    //{
    //    private IYearlyAverageElectricEnergyProductionPriceFactory _yaepFactory;
    //    private MonthlyPeriod _monthlyPeriod;
    //    private readonly Mock<IIdentityFactory<Guid>> _identityFactory;

    //    public YearlyAverageElectricEnergyProductionPriceFactoryTests()
    //    {
    //        _identityFactory = new Mock<IIdentityFactory<Guid>>();
    //        _monthlyPeriod = new MonthlyPeriod(DateTime.Now.AddYears(-1));
    //        _yaepFactory = new YearlyAverageElectricEnergyProductionPriceFactory(
    //            new YearlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
    //            new List<MonthlyAverageElectricEnergyProductionPrice>
    //            {
    //                new MonthlyAverageElectricEnergyProductionPrice(
    //                    1M,
    //                    nameof(MonthlyAverageElectricEnergyProductionPrice),
    //                    _monthlyPeriod,
    //                    _identityFactory.Object)
    //            },
    //            _identityFactory.Object);
    //    }

    //    public void MaepsMustBeSet()
    //    {
    //        Action action = () => new YearlyAverageElectricEnergyProductionPriceFactory(
    //            new YearlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2)),
    //            null,
    //            _identityFactory.Object);

    //        action
    //            .ShouldThrowExactly<DomainException>()
    //            .WithMessage(Infrastructure.Parameter.PeriodValidTillNotSetException);
    //    }

    //    public void MaepsMustContainValues()
    //    {
    //        Action action = () => new YearlyAverageElectricEnergyProductionPriceFactory(
    //            null,
    //            new List<MonthlyAverageElectricEnergyProductionPrice> { },
    //            _identityFactory.Object);

    //        action
    //            .ShouldThrowExactly<DomainException>()
    //            .WithMessage(Infrastructure.Parameter.PeriodValidTillNotSetException);
    //    }

    //    public void MaepsMustAllBeActiveAtCurrentMonth()
    //    {
    //        var name = nameof(MonthlyAverageElectricEnergyProductionPrice);
    //        var monthlyPeriod = new MonthlyPeriod(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2));

    //        Action action = () => new YearlyAverageElectricEnergyProductionPriceFactory(
    //            null,
    //            new List<MonthlyAverageElectricEnergyProductionPrice>
    //            {
    //                new MonthlyAverageElectricEnergyProductionPrice(
    //                    1M,
    //                    name,
    //                    monthlyPeriod,
    //                    _identityFactory.Object)
    //            },
    //            _identityFactory.Object);

    //        action
    //            .ShouldThrowExactly<DomainException>()
    //            .WithMessage(Infrastructure.Parameter.PeriodValidTillNotSetException);
    //    }

    //    public void LastYaepPeriodMustBeWithinInitialPeriodAndCurrentPeriod()
    //    {
    //        IYearlyAverageElectricEnergyProductionPriceFactory yaepFactory = new YearlyAverageElectricEnergyProductionPriceFactory(
    //            new YearlyPeriod(DateTime.Now.AddYears(-2), DateTime.Now.AddYears(-1)),
    //            new List<MonthlyAverageElectricEnergyProductionPrice>
    //            {
    //                new MonthlyAverageElectricEnergyProductionPrice(
    //                    1M,
    //                    nameof(MonthlyAverageElectricEnergyProductionPrice),
    //                    _monthlyPeriod,
    //                    _identityFactory.Object)
    //            },
    //            _identityFactory.Object);

    //        Action action = () => yaepFactory.CreateNew();

    //        action
    //            .ShouldThrowExactly<DomainException>()
    //            .WithMessage(string.Format(
    //                Infrastructure.Parameter.YaepDateException,
    //                DateTime.Parse(Infrastructure.Parameter.InitialPeriod),
    //                SystemTime.CurrentYear()));
    //    }

    //    public void YaepCanContainOnlyMaepsFromYaepPeriodYear()
    //    {
    //    }

    //    public void AmountIsProperlyCalculated()
    //    { }
    //}
}