using System;

namespace Acme.Seps.Presentation.Web.Test.Integration.TestUtility
{
    public class TestPriorityAttribute : Attribute
    {
        public int Priority { get; set; }
        public TestPriorityAttribute(int Priority)
        {
            this.Priority = Priority;
        }
    }
}