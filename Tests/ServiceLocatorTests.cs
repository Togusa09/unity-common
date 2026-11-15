using NUnit.Framework;
using Anarksoft.SharedCore.DI;

namespace Anarksoft.SharedCore.Tests
{
    public class ServiceLocatorTests
    {
        private interface ITestService
        {
            string GetName();
        }

        private class TestService : ITestService
        {
            public string GetName() => "TestService";
        }

        private class AnotherTestService
        {
            public string Value { get; } = "AnotherValue";
        }

        [SetUp]
        public void Setup()
        {
            // Reset the singleton before each test
            ServiceLocator.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            ServiceLocator.Reset();
        }

        [Test]
        public void Container_Singleton_ReturnsSameInstance()
        {
            var container1 = ServiceLocator.Container;
            var container2 = ServiceLocator.Container;

            Assert.AreSame(container1, container2, "Container should return the same singleton instance.");
        }

        [Test]
        public void Register_ValidService_RegistersSuccessfully()
        {
            var container = ServiceLocator.Container;
            var service = new TestService();

            container.Register<ITestService>(service);

            var resolved = container.Resolve<ITestService>();
            Assert.AreSame(service, resolved, "Registered service should be retrievable.");
        }

        [Test]
        public void Resolve_RegisteredService_ReturnsCorrectInstance()
        {
            var container = ServiceLocator.Container;
            var service = new TestService();

            container.Register<ITestService>(service);
            var resolved = container.Resolve<ITestService>();

            Assert.AreEqual("TestService", resolved.GetName(), "Resolved service should work correctly.");
        }

        [Test]
        public void Resolve_UnregisteredService_ThrowsKeyNotFoundException()
        {
            var container = ServiceLocator.Container;

            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(
                () => container.Resolve<ITestService>(),
                "Should throw KeyNotFoundException when resolving unregistered service."
            );
        }

        [Test]
        public void Register_OverwriteExistingService_ReplacesService()
        {
            var container = ServiceLocator.Container;
            var service1 = new TestService();
            var service2 = new TestService();

            container.Register<ITestService>(service1);
            container.Register<ITestService>(service2);

            var resolved = container.Resolve<ITestService>();
            Assert.AreSame(service2, resolved, "Should resolve the most recently registered service.");
        }

        [Test]
        public void TryResolve_RegisteredService_ReturnsTrueAndService()
        {
            var container = ServiceLocator.Container;
            var service = new TestService();

            container.Register<ITestService>(service);
            var success = container.TryResolve<ITestService>(out var resolved);

            Assert.IsTrue(success, "TryResolve should return true for registered service.");
            Assert.AreSame(service, resolved, "Should resolve the correct service.");
        }

        [Test]
        public void TryResolve_UnregisteredService_ReturnsFalse()
        {
            var container = ServiceLocator.Container;

            var success = container.TryResolve<ITestService>(out var resolved);

            Assert.IsFalse(success, "TryResolve should return false for unregistered service.");
            Assert.IsNull(resolved, "Resolved service should be null when not found.");
        }

        [Test]
        public void Unregister_RegisteredService_RemovesService()
        {
            var container = ServiceLocator.Container;
            var service = new TestService();

            container.Register<ITestService>(service);
            container.Unregister<ITestService>();

            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(
                () => container.Resolve<ITestService>(),
                "Should not be able to resolve unregistered service."
            );
        }

        [Test]
        public void Unregister_UnregisteredService_DoesNotThrow()
        {
            var container = ServiceLocator.Container;

            Assert.DoesNotThrow(() => container.Unregister<ITestService>(), 
                "Should not throw when unregistering non-existent service.");
        }

        [Test]
        public void Register_MultipleServiceTypes_ResolvesCorrectly()
        {
            var container = ServiceLocator.Container;
            var testService = new TestService();
            var anotherService = new AnotherTestService();

            container.Register<ITestService>(testService);
            container.Register<AnotherTestService>(anotherService);

            var resolvedTest = container.Resolve<ITestService>();
            var resolvedAnother = container.Resolve<AnotherTestService>();

            Assert.AreSame(testService, resolvedTest, "Should resolve correct test service.");
            Assert.AreSame(anotherService, resolvedAnother, "Should resolve correct another service.");
        }

        [Test]
        public void Reset_ClearsAllServices()
        {
            var container1 = ServiceLocator.Container;
            var service = new TestService();

            container1.Register<ITestService>(service);

            ServiceLocator.Reset();

            var container2 = ServiceLocator.Container;
            Assert.AreNotSame(container1, container2, "Container should be a new instance after reset.");
            
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(
                () => container2.Resolve<ITestService>(),
                "New container should not have previous registrations."
            );
        }

        [Test]
        public void Register_ValueType_RegistersAndResolvesCorrectly()
        {
            var container = ServiceLocator.Container;
            const int testValue = 42;

            container.Register<int>(testValue);
            var resolved = container.Resolve<int>();

            Assert.AreEqual(testValue, resolved, "Should register and resolve value types correctly.");
        }

        [Test]
        public void TryResolve_MultipleUnregisteredTypes_AllReturnFalse()
        {
            var container = ServiceLocator.Container;

            var success1 = container.TryResolve<ITestService>(out _);
            var success2 = container.TryResolve<AnotherTestService>(out _);

            Assert.IsFalse(success1, "First TryResolve should return false.");
            Assert.IsFalse(success2, "Second TryResolve should return false.");
        }
    }
}
