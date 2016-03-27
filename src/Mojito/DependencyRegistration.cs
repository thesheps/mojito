using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Mojito
{
    public interface IDependencyRegistration
    {
        T Resolve<T>();
        IDependencyRegistration WithArgument<T>(T value);
        IDependencyRegistration WithConnectionString(string connectionStringName);
        IDependencyRegistration WithAppSetting<T>(string settingName);
    }

    public class DependencyRegistration : IDependencyRegistration, IMojitoContainer
    {
        public DependencyRegistration(Type type, IMojitoContainer mojitoContainer)
        {
            _mojitoContainer = mojitoContainer;
            _factory = () => Activator.CreateInstance(type, _constructorArguments.ToArray());
        }

        public DependencyRegistration(Func<object> factory, IMojitoContainer mojitoContainer)
        {
            _factory = factory;
            _mojitoContainer = mojitoContainer;
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

        public T Resolve<T>(string name)
        {
            return _mojitoContainer.Resolve<T>(name);
        }

        public T Resolve<T>()
        {
            return (T)_factory.Invoke();
        }

        public IDependencyRegistration WithArgument<T>(T value)
        {
            _constructorArguments.Add(value);
            return this;
        }

        public IDependencyRegistration WithConnectionString(string connectionStringName)
        {
            _constructorArguments.Add(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString);
            return this;
        }

        public IDependencyRegistration WithAppSetting<T>(string settingName)
        {
            var value = GetAppSetting<T>(settingName);
            _constructorArguments.Add(value);

            return this;
        }

        private static T GetAppSetting<T>(string settingName)
        {
            var value = ConfigurationManager.AppSettings[settingName];
            if (typeof(T) == typeof(Uri))
                return ((T)(object)new Uri(value));

            return (T)Convert.ChangeType(value, typeof(T));
        }

        private readonly Func<object> _factory;
        private readonly IList<object> _constructorArguments = new List<object>();
        private readonly IMojitoContainer _mojitoContainer;
    }
}