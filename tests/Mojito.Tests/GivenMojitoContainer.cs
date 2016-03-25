using System;
using Mojito.Exceptions;
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
            var result = container.Resolve<TestClassA>();

            Assert.That(result.GetType(), Is.EqualTo(typeof(TestClassA)));
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void WhenIBindAnInterfaceToASingleConcreteType_ThenItCanSubsequentlyBeResolved()
        {
            var container = new MojitoContainer();
            container.Register<ITestClass, TestClassA>();

            var result = container.Resolve<ITestClass>();
            Assert.That(result.GetType(), Is.EqualTo(typeof(TestClassA)));
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void WhenIBindAnInterfaceToMultipleConcreteTypesWithoutNames_ThenADuplicateRegistrationExceptionIsThrown()
        {
            var container = new MojitoContainer();
            container.Register<ITestClass, TestClassA>();
            Assert.Throws<DuplicateRegistrationException>(() => { container.Register<ITestClass, TestClassB>(); });
        }

        [Test]
        public void WhenIBindAnInterfaceToMultipleConcreteTypesWithNamedRegistrations_ThenICanDistinguishBetweenEachImplementation()
        {
            var container = new MojitoContainer();
            container.Register<ITestClass, TestClassA>("A");
            container.Register<ITestClass, TestClassB>("B");

            var a = container.Resolve<ITestClass>("A");
            var b = container.Resolve<ITestClass>("B");

            Assert.That(a.GetType(), Is.EqualTo(typeof(TestClassA)));
            Assert.That(b.GetType(), Is.EqualTo(typeof(TestClassB)));
        }

        [Test]
        public void WhenIBindASingletonInstance_ThenConsecutiveResolutionsReturnTheSameInstance()
        {
            var container = new MojitoContainer();
            container.Singleton<ITestClass, TestClassA>(new TestClassA());

            var a = container.Resolve<ITestClass>();
            var b = container.Resolve<ITestClass>();

            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void WhenIBindAnInterfaceToMultipleConcreteTypesWithNamedRegistrationsAndAttemptToResolveWithoutName_ThenUnknownRegistrationExceptionIsThrown()
        {
            var container = new MojitoContainer();
            container.Register<ITestClass, TestClassA>("A");
            container.Register<ITestClass, TestClassB>("B");

            Assert.Throws<UnknownRegistrationException>(() => container.Resolve<ITestClass>());
        }

        [Test]
        public void WhenIBindAConcreteInstanceToDerivedConcreteTypesWithNamedRegistrationsAndAttemptToResolveWithoutName_ThenUnknownRegistrationExceptionIsThrown()
        {
            var container = new MojitoContainer();
            container.Register<TestClassA, TestClassA>("A");
            container.Register<TestClassA, TestClassC>("B");

            var results = container.Resolve<TestClassA>();
            Assert.That(results, Is.Not.Null);
        }

        [Test]
        public void WhenIResolveAnInstanceWithNoParameterlessConstructor_ThenCouldNotResolveRegistrationExceptionIsThrown()
        {
            var container = new MojitoContainer();
            container.Register<ITestClass, TestClassD>();

            Assert.Throws<MissingMethodException>(() => container.Resolve<ITestClass>());
        }

        //[Test]
        //public void WhenIResolveAnInstanceAndSpecifyCorrectlyNamedConstructorArgument_ThenInstanceIsCorrectlyInstantiated()
        //{
        //    var container = new MojitoContainer();
        //    container.Register<ITestClass, TestClassD>()
        //        .WithConstructorArgument("test", "Test");

        //    var result = (TestClassD)container.Resolve<ITestClass>();
        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.Test, Is.EqualTo("Test"));
        //}
    }
}