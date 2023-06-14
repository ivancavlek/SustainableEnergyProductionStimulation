using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Text;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity;

public class EconometricIndexTests
{
    private readonly decimal _amount;
    private readonly string _remark;
    private readonly DateTimeOffset _activeFrom;
    private readonly IIdentityFactory<Guid> _identityFactory;

    public EconometricIndexTests()
    {
        _amount = 1M;
        _remark = nameof(_remark);
        _activeFrom = DateTime.UtcNow;
        _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
    }

    public void AmountCannotBeANegativeValue()
    {
        Action action = () => new DummyEconometricIndex(-1M, _remark, _activeFrom, _identityFactory);

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.ValueZeroOrAbove("amount"));
    }

    public void AmountCannotBeZeroBased()
    {
        Action action = () => new DummyEconometricIndex(0M, _remark, _activeFrom, _identityFactory);

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.ValueZeroOrAbove("amount"));
    }

    public void RemarkMustExist()
    {
        Action action = () => new DummyEconometricIndex(_amount, string.Empty, _activeFrom, _identityFactory);

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.EntityNotSet("remark"));
    }

    public void CreatesProperly()
    {
        const decimal amount = 1.2578M;

        var econometricIndex = new DummyEconometricIndex(amount, _remark, _activeFrom, _identityFactory);

        econometricIndex.Remark.Should().Be(_remark);
        econometricIndex.Amount.Should().Be(Math.Round(amount, 2, MidpointRounding.AwayFromZero));
    }

    public void AmountCorrectionCannotBeDoneOnInitialValues()
    {
        var econometricIndex = new DummyEconometricIndex(
            _amount, _remark, SepsVersion.InitialDate(), _identityFactory);

        Action action = () => econometricIndex.AmountCorrection(1, "Test");

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.InitialValuesMustNotBeChanged());
    }

    public void AmountCorrectionCannotBeANegativeValue()
    {
        var econometricIndex = new DummyEconometricIndex(_amount, _remark, _activeFrom, _identityFactory);

        Action action = () => econometricIndex.AmountCorrection(-1, "Test");

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.ValueZeroOrAbove("amount"));
    }

    public void AmountCorrectionCannotBeZeroBased()
    {
        var econometricIndex = new DummyEconometricIndex(_amount, _remark, _activeFrom, _identityFactory);

        Action action = () => econometricIndex.AmountCorrection(0, "Test");

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.ValueZeroOrAbove("amount"));
    }

    public void AmountCorrectionRemarkMustExist()
    {
        var econometricIndex = new DummyEconometricIndex(_amount, _remark, _activeFrom, _identityFactory);

        Action action = () => econometricIndex.AmountCorrection(1, null);

        action
            .Should()
            .ThrowExactly<DomainException>()
            .WithMessage(SepsMessage.EntityNotSet("remark"));
    }

    public void AmountCorrectionIsCorrect()
    {
        const decimal amount = 1.2578M;

        var econometricIndex = new DummyEconometricIndex(1, "Test", _activeFrom, _identityFactory);

        econometricIndex.AmountCorrection(amount, _remark);

        econometricIndex.Remark.Should().Be(_remark);
        econometricIndex.Amount.Should().Be(Math.Round(amount, 2, MidpointRounding.AwayFromZero));
    }

    private class DummyEconometricIndex : EconometricIndex
    {
        protected override int DecimalPlaces => 2;

        public DummyEconometricIndex(
            decimal amount, string remark, DateTimeOffset since, IIdentityFactory<Guid> identityFactory)
            : base(amount, remark, since, identityFactory) { }

        internal new void AmountCorrection(decimal amount, string remark) => base.AmountCorrection(amount, remark);
    }
}