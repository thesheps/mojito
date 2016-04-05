using System;
using System.Collections.Generic;
using System.Linq;

namespace Mojito
{
    public class From
    {
        public static IList<IMojitoInstaller> AssemblyContaining<T>()
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