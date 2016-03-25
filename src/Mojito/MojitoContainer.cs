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
            return Register(typeof(T1), name, () => implementation);
        }

        public IDependencyRegistration Register<T1, T2>() where T2 : T1
        {
            return Register<T1, T2>(string.Empty);
        }

        public IDependencyRegistration Register<T1, T2>(string name) where T2 : T1
        {
            return Register(typeof(T1), name, () => Activator.CreateInstance<T2>());
        }

        public IDependencyRegistration Register<T>(Func<object> factory)
        {
            return Register<T>(factory, string.Empty);
        }

        public IDependencyRegistration Register<T>(Func<object> factory, string name)
        {
            return Register(typeof(T), name, factory);
        }

        public T Resolve<T>()
        {
            return Resolve<T>(string.Empty);
        }

        public T Resolve<T>(string name)
        {
            var key = new Tuple<Type, string>(typeof(T), name);
            IDependencyRegistration dependencyRegistration;

            if (_registrations.TryGetValue(key, out dependencyRegistration)) return dependencyRegistration.Resolve<T>();

            if (typeof(T).IsInterface || typeof(T).IsAbstract)
                throw new UnknownRegistrationException(nameof(T));

            dependencyRegistration = _registrations[key] = new DependencyRegistration(() => Activator.CreateInstance<T>());

            return dependencyRegistration.Resolve<T>();
        }

        private IDependencyRegistration Register(Type type, string name, Func<object> factory = null)
        {
            try
            {
                var dependencyRegistration = new DependencyRegistration(factory);
                _registrations.Add(Tuple.Create(type, name), dependencyRegistration);

                return dependencyRegistration;
            }
            catch (ArgumentException)
            {
                throw new DuplicateRegistrationException(nameof(type));
            }
        }

        private readonly Dictionary<Tuple<Type, string>, IDependencyRegistration> _registrations = new Dictionary<Tuple<Type, string>, IDependencyRegistration>();
    }
}