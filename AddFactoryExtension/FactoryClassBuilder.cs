using AddFactoryExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AddFactoryExtension
{
    /// <summary>
    /// Factory type interface structure for type validation and preparation for the IL type build
    /// </summary>
    internal class FactoryClassBuilder
    {
        private FactoryClassBuilder() { }

        /// <summary>
        /// Private readonly fields that should be generated inside the factory type
        /// </summary>
        public List<FactoryClassReadOnlyField> PrivateReadonlyFields { get; } = new List<FactoryClassReadOnlyField>();

        /// <summary>
        /// Factory methods that should be generated inside the factory type
        /// </summary>
        public List<FactoryClassMethod> FactoryMethods { get; } = new List<FactoryClassMethod>();

        /// <summary>
        /// Constructs the <see cref="FactoryClassBuilder"/> structure for later
        /// processing of the <see cref="ILFactoryTypeCreator"/>.
        /// </summary>
        /// <typeparam name="TFactory">Type of the factory interface</typeparam>
        /// <param name="assemblies">Assemblies to scan for implemented return types</param>
        /// <returns>New instance of <see cref="FactoryClassBuilder"/></returns>
        public static FactoryClassBuilder CreateFactoryClassBuilder<TFactory>(Assembly[] assemblies)
        {
            var facClsBuilder = new FactoryClassBuilder();
            IEnumerable<Type> allTypes = assemblies.SelectMany(t => t.GetTypes());
            IEnumerable<MethodInfo> factoryMethods = typeof(TFactory).GetMethods().Where(m => m.Name == "Factory");

            foreach (var factoryMethod in factoryMethods)
            {
                ParameterInfo[] factoryParams = factoryMethod.GetParameters();
                string debugName = $"{factoryMethod.DeclaringType.Name}.{factoryMethod.Name}({string.Join(",", factoryParams.Select(p => p.ParameterType.Name))})";

                Type interfaceReturnType = factoryMethod.ReturnType;
                if (!interfaceReturnType.IsInterface)
                    throw new FactoryMethodNotInterfaceException($"Factory method '{debugName}' return type must be an interface");

                List<Type> implementedTypes = allTypes.Where(t => interfaceReturnType.IsAssignableFrom(t) && !t.IsInterface).ToList();

                if (implementedTypes.Count == 0)
                    throw new NoImplementingClassFoundException($"No class implementing interface {interfaceReturnType.Name} found");

                List<ConstructorInfo> foundCtors = implementedTypes
                    .SelectMany(t => t.GetConstructors())
                    .ToList();

                List<ConstructorInfo> connectibleCtors = foundCtors.FindAll(fctor =>
                {
                    ParameterInfo[] fctorParams = fctor.GetParameters();

                    // no match if there are more factory method params than ctor params
                    if (fctorParams.Length < factoryParams.Length)
                        return false;

                    // no match if there is type mismatch at same parameter position
                    for (int i = 0; i < factoryParams.Length; i++)
                    {
                        if (fctorParams[i].ParameterType != factoryParams[i].ParameterType)
                            return false;
                    }

                    // for parameter positions AFTER all factory parameters,
                    // simple value type (ints, strings, enums, etc.) are not allowed 
                    // as it would result into an attempt to inject this type
                    // which is definitely not the right thing to do
                    for (int i = factoryParams.Length; i < fctorParams.Length; i++)
                    {
                        if (fctorParams[i].ParameterType.IsSimple())
                            return false;
                    }

                    //otherwise it is a match
                    return true;
                });

                if (connectibleCtors.Count == 0)
                    throw new NoValidCtorFoundException($"No valid constructors were found for factory method '{debugName}'. At least one constructor must match the factory parameters.");

                if (connectibleCtors.Count > 1)
                    throw new ConflictingCtorsFoundException($"There is more than one constructor for factory method '{debugName}'. Only one constructor is permitted.");

                ConstructorInfo ctor = connectibleCtors.Single();
                ParameterInfo[] ctorParams = ctor.GetParameters();

                var facCtorParameters = new List<FactoryClassMethodOrFieldParameter>();

                for (int i = 0; i < ctorParams.Length; i++)
                {
                    bool fromField = i > factoryParams.Length - 1;
                    bool isFactoryReinjection = ctorParams[i].ParameterType == typeof(TFactory);

                    string fieldName = $"field_{i}_{ctorParams[i].ParameterType.Name}";
                    string methodParamName = $"param_{i}_{ctorParams[i].ParameterType.Name}";

                    if (isFactoryReinjection)
                        fromField = false;

                    if (fromField)
                    {
                        facClsBuilder.PrivateReadonlyFields.Add(new FactoryClassReadOnlyField()
                        {
                            Name = fieldName,
                            Type = ctorParams[i].ParameterType
                        });
                    }

                    facCtorParameters.Add(new FactoryClassMethodOrFieldParameter()
                    {
                        IsFromField = fromField,
                        IsFactoryReinjection = isFactoryReinjection,
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