namespace Mojito.Tests.TestClasses
{
    public class TestClassE
    {
        public TestClassA TestClassA { get; }
        public TestClassB TestClassB { get; }
        public TestClassC TestClassC { get; }

        public TestClassE(TestClassA testClassA, TestClassB testClassB, TestClassC testClassC)
        {
            TestClassA = testClassA;
            TestClassB = testClassB;
            TestClassC = testClassC;
        }
    }
}