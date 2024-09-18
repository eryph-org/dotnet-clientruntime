#if NET462

using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using JetBrains.Annotations;

namespace Eryph.ClientRuntime.Configuration
{
    /// <summary>
    /// This module initializer will redirect all assemblies in case we deliver a newer version.
    /// It is required as System.Text.Json uses System.Buffer and System.CompilerServices.Unsafe
    /// in a minimal higher version then included with .net framework 462.
    /// </summary>
    [UsedImplicitly]
    public class UnsafeAssemblyHandler : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        private static readonly string AsmLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public void OnImport()
        {
            AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
        }

        public void OnRemove(PSModuleInfo psModuleInfo)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= HandleAssemblyResolve;
        }

        private static Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var requiredAssembly = new AssemblyName(args.Name);

            var possibleAssembly = Path.Combine(AsmLocation, $"{requiredAssembly.Name}.dll");

            AssemblyName bundledAssembly;
            try
            {
                bundledAssembly = AssemblyName.GetAssemblyName(possibleAssembly);
            }
            catch
            {
                return null;
            }

            if (bundledAssembly.Version < requiredAssembly.Version)
                return null;
            else
                return Assembly.LoadFrom(possibleAssembly);
        }
    }
}

#endif