using System;
using System.Collections.Generic;
using System.Linq;

namespace Mojito
{
    public interface IFluentInstaller
    {
        IList<IMojitoInstaller> AssemblyContaining<T>();
    }

    public class FluentInstaller : IFluentInstaller
    {
        public IList<IMojitoInstaller> AssemblyContaining<T>()
        {
            return typeof(T)
                .Assembly
                .GetExportedTypes()
                .Where(t => typeof(IMojitoInstaller).IsAssignableFrom(t))
                .Select(t => (IMojitoInstaller)Activator.CreateInstance(t))
                .ToList();
        }
    }
}