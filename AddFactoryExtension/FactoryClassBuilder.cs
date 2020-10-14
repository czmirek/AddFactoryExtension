using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection.AddFactoryExtension
{
    internal class FactoryClassBuilder
    {
        private FactoryClassBuilder() { }
        public List<FactoryClassReadOnlyField> PrivateReadonlyFields { get; } = new List<FactoryClassReadOnlyField>();
        public List<FactoryClassMethod> FactoryMethods { get; } = new List<FactoryClassMethod>();

        public static FactoryClassBuilder CreateFactoryClassBuilder<TFactory>(Assembly[] assemblies)
        {
            FactoryClassBuilder facClsBuilder = new FactoryClassBuilder();
            var allTypes = assemblies.SelectMany(t => t.GetTypes());
            var factoryMethods = typeof(TFactory).GetMethods().Where(m => m.Name == "Factory");

            foreach (var factoryMethod in factoryMethods)
            {
                var factoryParams = factoryMethod.GetParameters();
                string debugName = $"{factoryMethod.DeclaringType.Name}.{factoryMethod.Name}({string.Join(",", factoryParams.Select(p => p.ParameterType.Name))})";

                var interfaceReturnType = factoryMethod.ReturnType;
                if (!interfaceReturnType.IsInterface)
                    throw new InvalidOperationException($"Factory method '{debugName}' return type must be an interface");

                var foundCtors = allTypes
                    .Where(t => interfaceReturnType.IsAssignableFrom(t))
                    .SelectMany(t => t.GetConstructors());

                if (!foundCtors.Any())
                    throw new InvalidOperationException($"No constructors were found for factory method '{debugName}'");

                var connectibleCtors = foundCtors.Where(fctor =>
                {
                    var fctorParams = fctor.GetParameters();

                    //no match if there are more factory method params than ctor params
                    if (fctorParams.Length < factoryParams.Length)
                        return false;


                    for (int i = 0; i < factoryParams.Length; i++)
                    {
                        //no match if there is type mismatch at same parameter position
                        if (fctorParams[i].ParameterType != factoryParams[i].ParameterType)
                            return false;
                    }

                    //otherwise it is a match
                    return true;
                });

                if (!connectibleCtors.Any())
                    throw new InvalidOperationException($"No valid constructors were found for factory method '{debugName}'. At least one constructor must match the factory parameters.");

                if (connectibleCtors.Count() > 1)
                    throw new InvalidOperationException($"There is more than one constructor for factory method '{debugName}'. Only one constructor is permitted.");


                var ctor = connectibleCtors.Single();
                var ctorParams = ctor.GetParameters();

                var facCtorParameters = new List<FactoryClassMethodOrFieldParameter>();

                for (int i = 0; i < ctorParams.Length; i++)
                {
                    bool fromField = i > factoryParams.Length - 1;
                    string methodParamName = ctorParams[i].ParameterType.Name;
                    string fieldName = $"implFieldOfType_{ctorParams[i].ParameterType.Name}";

                    if (fromField)
                    {
                        facClsBuilder.PrivateReadonlyFields.Add(new FactoryClassReadOnlyField()
                        {
                            Name = $"implFieldOfType_{ctorParams[i].ParameterType.Name}",
                            Type = ctorParams[i].ParameterType
                        });
                    }

                    facCtorParameters.Add(new FactoryClassMethodOrFieldParameter()
                    {
                        IsFromField = fromField,
                        MatchingField = fieldName,
                        MethodParameterName = methodParamName,
                        Type = ctorParams[i].ParameterType
                    });
                }

                facClsBuilder.FactoryMethods.Add(new FactoryClassMethod()
                {
                    MatchingCtor = ctor,
                    Parameters = facCtorParameters,
                    FactoryInterfaceMethodInfo = factoryMethod
                });
            }

            return facClsBuilder;
        }
    }
}