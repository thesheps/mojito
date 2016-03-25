using System;
using System.Collections.Generic;

namespace Mojito
{
    public interface IMojitoContainer
    {
        IBindingExpression<T> Bind<T>();
        void Register<T>();
        void Register<T>(T implementation);
        void Register<T>(Func<object> factory);
        T Resolve<T>();
    }

    public class MojitoContainer : IMojitoContainer
    {
        public void Register<T>()
        {
            _registrations[typeof(T)] = () => Activator.CreateInstance<T>();
        }

        public void Register<T>(T implementation)
        {
            _registrations[typeof(T)] = () => Activator.CreateInstance(implementation.GetType());
        }

        public void Register<T>(Func<object> factory)
        {
            _registrations[typeof(T)] = factory;
        }

        public T Resolve<T>()
        {
            return (T)_registrations[typeof(T)].Invoke();
        }

        public IBindingExpression<T> Bind<T>()
        {
            return new BindingExpression<T>(this);
        }

        private readonly Dictionary<Type, Func<object>> _registrations = new Dictionary<Type, Func<object>>();
    }
}