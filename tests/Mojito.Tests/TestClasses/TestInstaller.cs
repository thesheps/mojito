namespace Mojito.Tests.TestClasses
{
    public class TestInstaller : IMojitoInstaller
    {
        public void Register(IMojitoContainer mojitoContainer)
        {
            mojitoContainer.Register<ITestClass, TestClassA>();
        }
    }
}