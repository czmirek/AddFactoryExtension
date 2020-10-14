using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection.AddFactoryExtension
{
    /// <summary>
    /// Activates the dynamically created factory type
    /// </summary>
    internal class FactoryTypeActivator
    {
        internal static object Activate(IServiceProvider sp, Type facType)
        {
            ConstructorInfo ctor = facType.GetConstructors().FirstOrDefault();
            ParameterInfo[] ctorParams = ctor.GetParameters();
            var ctorParamInstances = new List<object>();
            
            foreach (var ctorParam in ctorParams)
            {
                object instance = sp.GetRequiredService(ctorParam.ParameterType);
                ctorParamInstances.Add(instance);
            }

            return ctor.Invoke(ctorParamInstances.ToArray());
        }
    }
}