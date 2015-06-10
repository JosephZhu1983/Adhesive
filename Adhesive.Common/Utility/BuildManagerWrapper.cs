
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;

namespace Adhesive.Common
{
    public class BuildManagerWrapper
    {
        private static readonly BuildManagerWrapper current = new BuildManagerWrapper();
        private IEnumerable<Assembly> referencedAssemblies;
        private IEnumerable<Type> publicTypes;
        private IEnumerable<Type> concreteTypes;

        public static BuildManagerWrapper Current
        {
            [DebuggerStepThrough]
            get
            {
                return current;
            }
        }

        public virtual IEnumerable<Assembly> Assemblies
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    List<Assembly> allAssemblies = new List<Assembly>();
                    allAssemblies.Add(Assembly.GetEntryAssembly());
                    string path = AppDomain.CurrentDomain.BaseDirectory;
                    foreach (string dll in Directory.GetFiles(path, "*.dll"))
                    {
                        try
                        {
                            allAssemblies.Add(Assembly.LoadFrom(dll));
                        }
                        catch (Exception ex)
                        {
                            LocalLoggingService.Error(ex.ToString());
                        }
                    }

                    return referencedAssemblies ?? (referencedAssemblies = allAssemblies.Where(assembly => assembly != null && !assembly.GlobalAssemblyCache)
                        .Distinct(new LambdaComparer<Assembly>((x, y) => x.FullName == y.FullName)).ToList());
                }
                else
                {
                    return referencedAssemblies ?? (referencedAssemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().Where(assembly => !assembly.GlobalAssemblyCache).ToList());
                }
            }
        }

        public IEnumerable<Type> PublicTypes
        {
            get
            {
                return publicTypes ?? (publicTypes = Assemblies.PublicTypes().ToList());
            }
        }

        public IEnumerable<Type> ConcreteTypes
        {
            get
            {
                return concreteTypes ?? (concreteTypes = Assemblies.ConcreteTypes().ToList());
            }
        }
    }
}
