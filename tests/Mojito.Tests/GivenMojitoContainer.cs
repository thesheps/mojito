using System;
using System.Collections;
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

            Assert.Throws<CouldNotResolveRegistrationException>(() => container.Resolve<ITestClass>());
        }

        [Test]
        public void WhenIResolveAnInstanceAndSpecifyConstructorArgument_ThenInstanceIsCorrectlyInstantiated()
        {
            var container = new MojitoContainer();
            container.Register<ITestClass, TestClassD>()
                .WithArgument("test", "Test");

            var result = (TestClassD)container.Resolve<ITestClass>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Test, Is.EqualTo("Test"));
        }

        [Test]
        public void WhenIResolveAnInstanceAndSpecifyConnectionString_ThenInstanceIsCorrectlyInstantiated()
        {
            var container = new MojitoContainer();
            container.Register<ITestClass, TestClassD>()
                .WithConnectionString("test", "Test");

            var result = (TestClassD)container.Resolve<ITestClass>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Test, Is.EqualTo("Data Source=Test.db;Version=3;"));
        }

        [Test]
        public void WhenIResolveAnInstanceAndSpecifyStringAppSetting_ThenInstanceIsCorrectlyInstantiated()
        {
            var container = new MojitoContainer();
            container.Register<ITestClass, TestClassD>()
                .WithAppSetting<string>("test", "TestString");

            var result = (TestClassD)container.Resolve<ITestClass>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Test, Is.EqualTo("TestString"));
        }

        [Test]
        public void WhenIResolveAnInstanceAndSpecifyIntegerAppSetting_ThenInstanceIsCorrectlyInstantiated()
        {
            var container = new MojitoContainer();
            container.Register<ITestClass, TestClassD>()
                .WithAppSetting<int>("test", "TestInt");

            var result = (TestClassD)container.Resolve<ITestClass>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TestInt, Is.EqualTo(42));
        }

        [Test]
        public void WhenIResolveAnInstanceAndSpecifyDateTimeAppSetting_ThenInstanceIsCorrectlyInstantiated()
        {
            var container = new MojitoContainer();
            container.Register<ITestClass, TestClassD>()
                .WithAppSetting<DateTime>("test", "TestDateTime");

            var result = (TestClassD)container.Resolve<ITestClass>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TestDateTime, Is.EqualTo(DateTime.Parse("1985-09-11 11:00:00")));
        }

        [Test]
        public void WhenIResolveAnInstanceWithNonSpecifiedDependencies_ThenImpliedDependenciesAreResolvedUsingContainer()
        {
            var container = new MojitoContainer();
            var result = container.Resolve<TestClassE>();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.GetType(), Is.EqualTo(typeof(TestClassE)));
        }

        [Test]
        public void WhenIResolve2Instances_ThenEachInstanceIsUnique()
        {
            var container = new MojitoContainer();
            container.Register<ITestClass, TestClassA>();

            var a = container.Resolve<ITestClass>();
            var b = container.Resolve<ITestClass>();

            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void WhenIRegisterAGenericType_ThenInstanceCanBeResolved()
        {
            var container = new MojitoContainer();
            container.Register<ITestClassHandler<TestClassA>, TestClassAHandler>();

            var result = container.Resolve<ITestClassHandler<TestClassA>>();
            Assert.That(result.GetType(), Is.EqualTo(typeof(TestClassAHandler)));
        }

        [Test]
        public void WhenIResolveAGenericImplementationWithoutAutomaticRegistration_ThenUnknownRegistrationExceptionIsThrown()
        {
            var container = new MojitoContainer();
            Assert.Throws<UnknownRegistrationException>(() => container.Resolve<ITestClassHandler<TestClassA>>());
        }

        [Test]
        public void WhenIResolveAGenericImplementationWithAutomaticRegistration_ThenInstanceCanBeResolved()
        {
            var container = new MojitoContainer { UseAutomaticRegistration = true };
            var result = container.Resolve<ITestClassHandler<TestClassA>>();

            Assert.That(result.GetType(), Is.EqualTo(typeof(TestClassAHandler)));
        }

        [Test]
        public void WhenIResolveAGenericImplementationWithAutomaticRegistrationAndMultipleMatches_ThenDuplicateRegistrationExceptionIsThrown()
        {
            var container = new MojitoContainer { UseAutomaticRegistration = true };

            Assert.Throws<DuplicateRegistrationException>(() => container.Resolve<IDuplicateHandler<TestClassA>>());
        }

        [Test]
        public void WhenIResolveAGenericImplementationWithAutomaticRegistrationAndNoParameterlessConstructor_ThenInstanceCanBeResolved()
        {
            var container = new MojitoContainer { UseAutomaticRegistration = true };
            var result = container.Resolve<ITestClassHandler<TestClassA>>();

            Assert.That(result.GetType(), Is.EqualTo(typeof(TestClassAHandler)));
        }

        [Test]
        public void WhenIInstallDependenciesUsingAnInstaller_ThenTheRegistrationsCanSubsequentlyBeResolved()
        {
            var container = new MojitoContainer();
            container.Install(new TestInstaller());

            var a = container.Resolve<ITestClass>();
            Assert.That(a.GetType(), Is.EqualTo(typeof(TestClassA)));
        }

        [Test]
        public void WhenISpecifyInstallersByAssembly_ThenRegistrationsCanSubsequentlyBeResolved()
        {
            var container = new MojitoContainer();
            container.Install(From.AssemblyContaining<TestInstaller>());

            var a = container.Resolve<ITestClass>();
            Assert.That(a.GetType(), Is.EqualTo(typeof(TestClassA)));
        }
    }
}