using System;
using System.Collections.Generic;
using Mojito.Exceptions;

namespace Mojito
{
    public interface IMojitoContainer
    {
        void Singleton<T1, T2>(T2 implementation, string name) where T2 : T1;
        void Singleton<T1, T2>(T2 implementation) where T2 : T1;
        void Register<T1, T2>(string name);
        void Register<T1, T2>() where T2 : T1;
        void Register<T>(Func<object> factory, string name);
        void Register<T>(Func<object> factory);
        T Resolve<T>(string name);
        T Resolve<T>();
    }

    public class MojitoContainer : IMojitoContainer
    {
        public void Singleton<T1, T2>(T2 implementation) where T2 : T1
        {
            Singleton<T1, T2>(implementation, string.Empty);
        }

        public void Singleton<T1, T2>(T2 implementation, string name) where T2 : T1
        {
            Register(typeof(T1), name, () => implementation);
        }

        public void Register<T1, T2>() where T2 : T1
        {
            Register<T1, T2>(string.Empty);
        }

        public void Register<T1, T2>(string name)
        {
            Register(typeof(T1), name, () => Activator.CreateInstance<T2>());
        }

        public void Register<T>(Func<object> factory)
        {
            Register<T>(factory, string.Empty);
        }

        public void Register<T>(Func<object> factory, string name)
        {
            Register(typeof(T), name, factory);
        }

        public T Resolve<T>()
        {
            return Resolve<T>(string.Empty);
        }

        public T Resolve<T>(string name)
        {
            var key = new Tuple<Type, string>(typeof(T), name);
            Func<object> factory;

            if (_registrations.TryGetValue(key, out factory)) return (T) factory.Invoke();

            if (typeof(T).IsInterface || typeof(T).IsAbstract)
                throw new UnknownRegistrationException(nameof(T));

            factory = _registrations[key] = () => Activator.CreateInstance<T>();

            return (T)factory.Invoke();
        }

        private void Register(Type type, string name, Func<object> factory)
        {
            try
            {
                _registrations.Add(Tuple.Create(type, name), factory);
            }
            catch (ArgumentException)
            {
                throw new DuplicateRegistrationException(nameof(type));
            }
        }

        private readonly Dictionary<Tuple<Type, string>, Func<object>> _registrations = new Dictionary<Tuple<Type, string>, Func<object>>();
    }
}