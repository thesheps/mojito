using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Mojito.Exceptions;

namespace Mojito
{
    public interface IDependencyRegistration
    {
        object Resolve();
        IDependencyRegistration WithArgument<T>(string name, T value);
        IDependencyRegistration WithConnectionString(string name, string connectionStringName);
        IDependencyRegistration WithAppSetting<T>(string name, string settingName);
    }

    public class DependencyRegistration : IDependencyRegistration, IMojitoContainer
    {
        public bool UseAutomaticRegistration => _mojitoContainer.UseAutomaticRegistration;

        public DependencyRegistration(IMojitoContainer mojitoContainer, Func<object> factory)
        {
            _mojitoContainer = mojitoContainer;
            _factory = factory;
        }

        public DependencyRegistration(IMojitoContainer mojitoContainer, Type type)
        {
            _mojitoContainer = mojitoContainer;
            _factory = () =>
            {
                try
                {
                    var constructor = type
                        .GetConstructors()
                        .Select(c => new Constructor(c, c.GetParameters().Select(p => GetParameter(p.ParameterType, p.Name)).ToList()))
                        .OrderByDescending(p => p.Parameters.Count)
                        .FirstOrDefault(c => c.Parameters.All(p => p != null));

                    if (constructor == null)
                        throw new CouldNotResolveRegistrationException(type);

                    return constructor.Invoke();
                }
                catch (MissingMethodException)
                {
                    throw new CouldNotResolveRegistrationException(type);
                }
            };
        }

        public IDependencyRegistration Singleton<T1, T2>(T2 implementation) where T2 : T1
        {
            return _mojitoContainer.Singleton<T1, T2>(implementation);
        }

        public IDependencyRegistration Singleton<T1, T2>(T2 implementation, string name) where T2 : T1
        {
            return _mojitoContainer.Singleton<T1, T2>(implementation, name);
        }

        public IDependencyRegistration Register<T1, T2>() where T2 : T1
        {
            return _mojitoContainer.Register<T1, T2>();
        }

        public IDependencyRegistration Register<T1, T2>(string name) where T2 : T1
        {
            return _mojitoContainer.Register<T1, T2>(name);
        }

        public IDependencyRegistration Register<T>(Func<object> factory)
        {
            return _mojitoContainer.Register<T>(factory);
        }

        public IDependencyRegistration Register<T>(Func<object> factory, string name)
        {
            return _mojitoContainer.Register<T>(factory, name);
        }

        public object Resolve(Type type)
        {
            return _mojitoContainer.Resolve(type);
        }

        public object Resolve(Type type, string name)
        {
            return _mojitoContainer.Resolve(type, name);
        }

        public T Resolve<T>()
        {
            return _mojitoContainer.Resolve<T>();
        }

        public T Resolve<T>(string name)
        {
            return _mojitoContainer.Resolve<T>(name);
        }
        
        public object Resolve()
        {
            return _factory.Invoke();
        }

        public IDependencyRegistration WithArgument<T>(string name, T value)
        {
            _constructorArguments.Add(new Tuple<Type, string>(typeof(T), name), value);
            return this;
        }

        public IDependencyRegistration WithConnectionString(string name, string connectionStringName)
        {
            _constructorArguments.Add(new Tuple<Type, string>(typeof(string), name), ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString);
            return this;
        }

        public IDependencyRegistration WithAppSetting<T>(string name, string settingName)
        {
            var value = GetAppSetting<T>(settingName);
            _constructorArguments.Add(new Tuple<Type, string>(typeof(T), name), value);

            return this;
        }

        private object GetParameter(Type type, string name)
        {
            object parameter;
            if (_constructorArguments.TryGetValue(new Tuple<Type, string>(type, name), out parameter))
                return parameter;

            if (type.IsValueType || _ignoredTypes.Contains(type))
                return null;

            try
            {
                parameter = _mojitoContainer.Resolve(type);
            }
            catch (CouldNotResolveRegistrationException)
            {
                parameter = null;
            }

            return parameter;
        }

        private static T GetAppSetting<T>(string settingName)
        {
            var value = ConfigurationManager.AppSettings[settingName];
            if (typeof(T) == typeof(Uri))
                return ((T)(object)new Uri(value));

            return (T)Convert.ChangeType(value, typeof(T));
        }

        private readonly Dictionary<Tuple<Type, string>, object> _constructorArguments = new Dictionary<Tuple<Type, string>, object>();
        private readonly IList<Type> _ignoredTypes = new[] { typeof(string), typeof(DateTime), };
        private readonly IMojitoContainer _mojitoContainer;
        private readonly Func<object> _factory;

        private class Constructor
        {
            public IList<object> Parameters { get; }

            public Constructor(ConstructorInfo constructorInfo, IList<object> parameters)
            {
                _constructorInfo = constructorInfo;
                Parameters = parameters;
            }

            public object Invoke()
            {
                return _constructorInfo.Invoke(Parameters.ToArray());
            }

            private readonly ConstructorInfo _constructorInfo;
        }
    }
}