using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Subsidy.Entity;
using FluentAssertions;
using NSubstitute;
using System;
using System.Reflection;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class NaturalGasSellingPriceTests
    {
        private readonly decimal _amount;
        private readonly string _remark;
        private readonly IIdentityFactory<Guid> _identityFactory;
        private readonly NaturalGasSellingPrice _existingNaturalGasSellingPrice;

        public NaturalGasSellingPriceTests()
        {
            _amount = 1.123456M;
            _remark = nameof(_remark);
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();

            _existingNaturalGasSellingPrice = Activator.CreateInstance(
                typeof(NaturalGasSellingPrice),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    10M,
                    nameof(NaturalGasSellingPrice),
                    new Period(new MonthlyPeriodFactory(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2))),
                    _identityFactory },
                null) as NaturalGasSellingPrice;
        }

        public void NewNaturalGasSellingPriceIsProperlyConstructed()
        {
            var validTill = DateTime.Now.AddMonths(-5);

            var result = _existingNaturalGasSellingPrice.CreateNew(
                _amount, _remark, validTill.Month, validTill.Year, _identityFactory);

            result.Amount.Should().Be(Math.Round(_amount, 2, MidpointRounding.AwayFromZero));
            _existingNaturalGasSellingPrice.Period.ValidTill.Should().NotBeNull();
            _existingNaturalGasSellingPrice.Period.ValidTill.Value
                .Should().Be(new DateTimeOffset(new DateTime(validTill.Year, validTill.Month, 1)));
            result.Period.ValidFrom.Should().Be(new DateTimeOffset(new DateTime(validTill.Year, validTill.Month, 1)));
            result.Period.ValidTill.Should().BeNull();
        }
    }
}