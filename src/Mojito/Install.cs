using System;
using System.Collections.Generic;
using System.Linq;

namespace Mojito
{
    public interface IInstall
    {
        IList<IMojitoInstaller> FromAssemblyContaining<T>();
    }

    public class Install : IInstall
    {
        public IList<IMojitoInstaller> FromAssemblyContaining<T>()
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