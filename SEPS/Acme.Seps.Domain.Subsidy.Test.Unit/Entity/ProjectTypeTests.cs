using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Subsidy.Entity;
using FluentAssertions;
using System;
using System.Reflection;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class ProjectTypeTests
    {
        private readonly ProjectType _projectType;
        private readonly Period _period;

        public ProjectTypeTests()
        {
            _period = new Period(new MonthlyPeriodFactory(SystemTime.CurrentMonth().AddMonths(-6)));

            _projectType = Activator.CreateInstance(
                typeof(ProjectType),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                null,
                null) as ProjectType;

            typeof(ProjectType).BaseType.GetProperty("Period").SetValue(_projectType, _period);
        }

        public void ProjectTypeIsCorrectlyDisabled()
        {
            _projectType.PermanentlyDisable();

            _projectType.Period.ValidTill.Should().NotBeNull();
            _projectType.Period.ValidTill.Should().Be(SystemTime.CurrentMonth());
        }
    }
}