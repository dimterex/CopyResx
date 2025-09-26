using System.Reflection;
using System.Runtime.Loader;

namespace CopyToLocales
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public CustomAssemblyLoadContext() : base("Modules", true)
        { }
 
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}