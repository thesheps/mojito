using System;
using System.Collections.Generic;
using System.Linq;

namespace Mojito
{
    public interface IDependencyRegistration
    {
        T Resolve<T>();
        IDependencyRegistration WithConstructorArgument<T>(string argumentName, T value);
    }

    public class DependencyRegistration : IDependencyRegistration
    {
        public DependencyRegistration(Func<object> factory)
        {
            _factory = factory;
        }

        public T Resolve<T>()
        {
            if (_factory == null)
                _factory = () => Activator.CreateInstance(typeof(T), _constructorArguments.Select(c => c.Value).ToArray());

            return (T)_factory.Invoke();
        }

        public IDependencyRegistration WithConstructorArgument<T>(string argumentName, T value)
        {
            _constructorArguments[argumentName] = value;
            return this;
        }

        private Func<object> _factory;
        private readonly Dictionary<string, object> _constructorArguments = new Dictionary<string, object>();
    }
}