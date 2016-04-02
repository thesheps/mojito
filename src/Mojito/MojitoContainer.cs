using System;
using System.Collections.Generic;
using Mojito.Exceptions;

namespace Mojito
{
    public interface IMojitoContainer
    {
        IDependencyRegistration Singleton<T1, T2>(T2 implementation, string name) where T2 : T1;
        IDependencyRegistration Singleton<T1, T2>(T2 implementation) where T2 : T1;
        IDependencyRegistration Register<T1, T2>(string name) where T2 : T1;
        IDependencyRegistration Register<T1, T2>() where T2 : T1;
        IDependencyRegistration Register<T>(Func<object> factory, string name);
        IDependencyRegistration Register<T>(Func<object> factory);
        object Resolve(Type type, string name);
        object Resolve(Type type);
        T Resolve<T>(string name);
        T Resolve<T>();
    }

    public class MojitoContainer : IMojitoContainer
    {
        public IDependencyRegistration Singleton<T1, T2>(T2 implementation) where T2 : T1
        {
            return Singleton<T1, T2>(implementation, string.Empty);
        }

        public IDependencyRegistration Singleton<T1, T2>(T2 implementation, string name) where T2 : T1
        {
            var dependencyRegistration = new DependencyRegistration(this, () => implementation);
            AddRegistration(typeof(T1), name, dependencyRegistration);

            return dependencyRegistration;
        }

        public IDependencyRegistration Register<T1, T2>() where T2 : T1
        {
            return Register<T1, T2>(string.Empty);
        }

        public IDependencyRegistration Register<T1, T2>(string name) where T2 : T1
        {
            var dependencyRegistration = new DependencyRegistration(this, typeof(T2));
            AddRegistration(typeof(T1), name, dependencyRegistration);

            return dependencyRegistration;
        }

        public IDependencyRegistration Register<T>(Func<object> factory)
        {
            return Register<T>(factory, string.Empty);
        }

        public IDependencyRegistration Register<T>(Func<object> factory, string name)
        {
            var dependencyRegistration = new DependencyRegistration(this, typeof(T));
            AddRegistration(typeof(T), name, dependencyRegistration);

            return dependencyRegistration;
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T), string.Empty);
        }

        public T Resolve<T>(string name)
        {
            return (T)Resolve(typeof(T), name);
        }

        public object Resolve(Type type)
        {
            return Resolve(type, string.Empty);
        }

        public object Resolve(Type type, string name)
        {
            var key = new Tuple<Type, string>(type, name);
            IDependencyRegistration dependencyRegistration;

            if (_registrations.TryGetValue(key, out dependencyRegistration))
                return dependencyRegistration.Resolve();

            if (type.IsInterface || type.IsAbstract)
                throw new UnknownRegistrationException(type);

            dependencyRegistration = _registrations[key] = new DependencyRegistration(this, type);

            return dependencyRegistration.Resolve();
        }

        private void AddRegistration(Type type, string name, IDependencyRegistration dependencyRegistration)
        {
            try
            {
                _registrations.Add(new Tuple<Type, string>(type, name), dependencyRegistration);
            }
            catch (ArgumentException)
            {
                throw new DuplicateRegistrationException(type);
            }
        }

        private readonly Dictionary<Tuple<Type, string>, IDependencyRegistration> _registrations = new Dictionary<Tuple<Type, string>, IDependencyRegistration>();
    }
}