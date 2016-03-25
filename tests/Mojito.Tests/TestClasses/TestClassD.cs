namespace Mojito.Tests.TestClasses
{
    public class TestClassD : ITestClass
    {
        public string Test { get; }

        public TestClassD(string test)
        {
            Test = test;
        }
    }
}