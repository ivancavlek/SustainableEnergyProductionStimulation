using System;

namespace Acme.Seps.Presentation.Web.Test.Integration.TestUtility;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class TestPriorityAttribute : Attribute
{
    public int Priority { get; }

    public TestPriorityAttribute(int priority) =>
        Priority = priority;
}