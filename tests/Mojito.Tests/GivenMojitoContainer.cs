using Mojito.Tests.TestClasses;
using NUnit.Framework;

namespace Mojito.Tests
{
    public class GivenMojitoContainer
    {
        [Test]
        public void WhenIRegisterATypeWithAParameterlessConstructor_ThenItCanSubsequentlyBeResolved()
        {
            var container = new MojitoContainer();
            container.Register<TestClassA>();

            var result = container.Resolve<TestClassA>();
            Assert.That(result.GetType(), Is.EqualTo(typeof(TestClassA)));
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void WhenIBindAnInterfaceToASingleConcreteType_ThenItCanSubsequentlyBeResolved()
        {
            var container = new MojitoContainer();
            container.Bind<ITestClass>().To<TestClassA>();

            var result = container.Resolve<ITestClass>();
            Assert.That(result.GetType(), Is.EqualTo(typeof(TestClassA)));
            Assert.That(result, Is.Not.Null);
        }
    }
}