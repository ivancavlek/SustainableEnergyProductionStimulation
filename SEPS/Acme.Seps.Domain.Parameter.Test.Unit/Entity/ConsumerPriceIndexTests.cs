using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.Entity;
using FluentAssertions;
using Moq;
using System;
using System.Reflection;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class ConsumerPriceIndexTests
    {
        private readonly decimal _amount;
        private readonly string _remark;
        private readonly Mock<IIdentityFactory<Guid>> _identityFactory;
        private readonly ConsumerPriceIndex _existingCpi;

        public ConsumerPriceIndexTests()
        {
            _amount = 1.123456M;
            _remark = nameof(_remark);
            _identityFactory = new Mock<IIdentityFactory<Guid>>();

            _existingCpi = Activator.CreateInstance(
                typeof(ConsumerPriceIndex),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    10M,
                    nameof(ConsumerPriceIndex),
                    new YearlyPeriod(DateTime.Now.AddYears(-4), DateTime.Now.AddYears(-3)),
                    _identityFactory.Object },
                null) as ConsumerPriceIndex;
        }

        public void AmountIsProperlyRounded()
        {
            var result = _existingCpi.CreateNew(_amount, _remark, _identityFactory.Object);

            result.Amount.Should().Be(Math.Round(_amount, 4, MidpointRounding.AwayFromZero));
        }
    }
}