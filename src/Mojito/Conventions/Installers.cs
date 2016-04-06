using System;
using System.Collections.Generic;
using System.Linq;

namespace Mojito.Conventions
{
    public interface IInstallers
    {
        IList<IMojitoInstaller> FromAssemblyContaining<T>();
    }

    public class Installers : IInstallers
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