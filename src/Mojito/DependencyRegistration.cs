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
        public DependencyRegistration(Type type)
        {
            _factory = () => Activator.CreateInstance(type, _constructorArguments.Select(c => c.Value).ToArray());
        }

        public DependencyRegistration(Func<object> factory)
        {
            _factory = factory;
        }

        public T Resolve<T>()
        {
            return (T)_factory.Invoke();
        }

        public IDependencyRegistration WithConstructorArgument<T>(string argumentName, T value)
        {
            _constructorArguments[argumentName] = value;
            return this;
        }

        private readonly Func<object> _factory;
        private readonly Dictionary<string, object> _constructorArguments = new Dictionary<string, object>();
    }
}