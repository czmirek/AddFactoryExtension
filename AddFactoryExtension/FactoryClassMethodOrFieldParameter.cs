using System;

namespace Microsoft.Extensions.DependencyInjection.AddFactoryExtension
{
    internal class FactoryClassMethodOrFieldParameter
    {
        public Type Type { get; set; }
        public bool IsFromField { get; set; }
        public string MethodParameterName { get; set; }
        public string MatchingField { get; set; }
    }
}