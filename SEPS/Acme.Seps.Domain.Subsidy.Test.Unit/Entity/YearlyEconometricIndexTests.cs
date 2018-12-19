using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Text;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class YearlyEconometricIndexTests
    {
        private readonly decimal _amount;
        private readonly string _remark;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public YearlyEconometricIndexTests()
        {
            _amount = 1M;
            _remark = nameof(_remark);
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
        }

        public void DateCannotBeFromCurrentYear()
        {
            var currentYear = DateTime.Now.Date;

            Action action = () => new DummyYearlyEconometricIndex(
                _amount, _remark, currentYear, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.YearlyParameterException);
        }

        public void DateIsCorrectlySet()
        {
            DateTimeOffset yearBeforeCurrentYear = DateTime.Now.Date.AddYears(-1);

            var econometricIndex = new DummyYearlyEconometricIndex(
                _amount, _remark, yearBeforeCurrentYear, _identityFactory);

            econometricIndex.Active.Since.Should().Be(yearBeforeCurrentYear.ToFirstDayOfTheYear());
        }

        public void YearlyEconometricIndexIsCorrected()
        {
            DateTimeOffset yearBeforeCurrentYear = DateTime.Now.Date.AddYears(-1);

            var econometricIndex = new DummyYearlyEconometricIndex(
                _amount, _remark, yearBeforeCurrentYear, _identityFactory);

            const decimal amount = 20M;
            const string remark = "Test";

            econometricIndex.Correct(amount, remark);

            econometricIndex.Amount.Should().Be(Math.Round(amount, 2, MidpointRounding.AwayFromZero));
            econometricIndex.Remark.Should().Be(remark);
        }

        private class DummyYearlyEconometricIndex : YearlyEconometricIndex<DummyYearlyEconometricIndex>
        {
            protected override int DecimalPlaces => 2;

            public DummyYearlyEconometricIndex(
                decimal amount, string remark, DateTimeOffset since, IIdentityFactory<Guid> identityFactory)
                : base(amount, remark, since, identityFactory) { }
        }
    }
}