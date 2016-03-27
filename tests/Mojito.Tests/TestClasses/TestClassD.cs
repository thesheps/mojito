using System;

namespace Mojito.Tests.TestClasses
{
    public class TestClassD : ITestClass
    {
        public string Test { get; }
        public int TestInt { get; }
        public DateTime TestDateTime { get; }

        public TestClassD(string test)
        {
            Test = test;
        }

        public TestClassD(int test)
        {
            TestInt = test;
        }

        public TestClassD(DateTime test)
        {
            TestDateTime = test;
        }
    }
}