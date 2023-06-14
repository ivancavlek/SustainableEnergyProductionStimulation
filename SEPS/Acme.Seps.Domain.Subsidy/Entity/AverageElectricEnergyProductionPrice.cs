using Acme.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity;

public class AverageElectricEnergyProductionPrice
    : MonthlyEconometricIndex<AverageElectricEnergyProductionPrice>
{
    protected override int DecimalPlaces => 4;

    protected AverageElectricEnergyProductionPrice() { }

    internal protected AverageElectricEnergyProductionPrice(
        decimal amount, string remark, DateTimeOffset since, IIdentityFactory<Guid> guidIdentityFactory)
        : base(amount, remark, since, guidIdentityFactory) { }
}