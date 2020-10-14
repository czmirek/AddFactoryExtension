using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection.AddFactoryExtension
{
    internal class FactoryTypeActivator
    {
        internal static object Activate(IServiceProvider sp, Type facType)
        {
            var ctor = facType.GetConstructors().FirstOrDefault();
            var ctorParams = ctor.GetParameters();
            var ctorParamInstances = new List<object>();
            
            foreach (var ctorParam in ctorParams)
            {
                object instance = sp.GetRequiredService(ctorParam.ParameterType);
                ctorParamInstances.Add(instance);
            }

            return Activator.CreateInstance(facType, ctorParamInstances);
        }
    }
}