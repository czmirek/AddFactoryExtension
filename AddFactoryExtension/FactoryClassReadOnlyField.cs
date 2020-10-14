using System;

namespace Microsoft.Extensions.DependencyInjection.AddFactoryExtension
{
    internal class FactoryClassReadOnlyField
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }
}