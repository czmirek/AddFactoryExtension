namespace AddFactoryExtension
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;

    /// <summary>
    /// IL type creator of the factory implementation type
    /// </summary>
    /// <remarks>
    /// Example implementation type should look like this:
    /// 
    /// public class DynamicFactoryForIMyFactory : IMyFactory 
    /// {
    ///     // these fields are emitted only if the services
    ///     // in their ctors require these values
    ///     private readonly ISomeService someService;
    ///     public DynamicFactoryForIMyFactory(ISomeService someService) 
    ///     {
    ///         this.someService = someService;
    ///     }
    ///     
    ///     public IFoo Factory() 
    ///     {
    ///         return new Foo();
    ///     }
    ///     
    ///     // parameters should be automatically assigned to the matching ctor
    ///     public IFoo Factory(string param1) 
    ///     {
    ///         return new Foo(param1);
    ///     }
    ///     
    ///     // parameters should be automatically assigned to the matching ctor
    ///     // also with the resolved service
    ///     public IBar Factory(int param1) 
    ///     {
    ///         return new Bar(param1, this.someService);
    ///     }
    /// }
    /// 
    /// </remarks>
    internal static class ILFactoryTypeCreator
    {
        public static Type CreateType<TFactory>(FactoryClassBuilder builder) where TFactory : class
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            // assembly stuff
            AppDomain appDomain = Thread.GetDomain();
            AssemblyName asmName = new AssemblyName { Name = "ToFactoryDynamicAssembly" };
            AssemblyBuilder asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            
            ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule("ToFactoryModule");
            Type objType = Type.GetType("System.Object");
            ConstructorInfo objCtor = objType.GetConstructor(new Type[0]);

            // type construction
            TypeBuilder facBuilder = moduleBuilder.DefineType($"DynamicFactoryFor{typeof(TFactory).Name}",
                TypeAttributes.Public
                | TypeAttributes.AutoClass
                | TypeAttributes.AnsiClass
                | TypeAttributes.BeforeFieldInit);

            facBuilder.AddInterfaceImplementation(typeof(TFactory));

            var fieldBuilders = new Dictionary<string, FieldBuilder>();
            foreach (var item in builder.PrivateReadonlyFields)
            {
                FieldBuilder fieldBuilder = facBuilder.DefineField(item.Name, item.Type, FieldAttributes.Private | FieldAttributes.InitOnly);
                fieldBuilders.Add(item.Name, fieldBuilder);
            }

            // ctor construction
            var ctorParams = builder.PrivateReadonlyFields.Select(t => t.Type).ToArray();
            ConstructorBuilder facCtorBuilder = facBuilder.DefineConstructor(
                MethodAttributes.Public
                | MethodAttributes.HideBySig
                | MethodAttributes.SpecialName
                | MethodAttributes.RTSpecialName, CallingConventions.Standard, ctorParams);

            ILGenerator ctorIL = facCtorBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, objCtor);
            //ctorIL.Emit(OpCodes.Nop);

            // generate IL code for assigning the fields with types (parsed from the resolved type constructors) 
            // resolved by the IoC container
            
            for (int i = 0; i < builder.PrivateReadonlyFields.Count; i++)
            {
                ctorIL.Emit(OpCodes.Ldarg_0);

                if (i == 0)
                    ctorIL.Emit(OpCodes.Ldarg_1);
                else if (i == 1)
                    ctorIL.Emit(OpCodes.Ldarg_2);
                else if (i == 2)
                    ctorIL.Emit(OpCodes.Ldarg_3);
                else
                    ctorIL.Emit(OpCodes.Ldarg_S, builder.PrivateReadonlyFields[i].Name);

                ctorIL.Emit(OpCodes.Stfld, fieldBuilders[builder.PrivateReadonlyFields[i].Name]);
            }

            ctorIL.Emit(OpCodes.Ret);


            // build the factory methods
            foreach (var facMethod in builder.FactoryMethods)
            {
                var types = facMethod.Parameters
                                     .Where(p => !p.IsFromField && !p.IsFactoryReinjection)
                                     .Select(p => p.Type)
                                     .ToArray();

                var methodBuilder = facBuilder.DefineMethod("Factory",
                    MethodAttributes.Public
                    | MethodAttributes.HideBySig
                    | MethodAttributes.NewSlot
                    | MethodAttributes.Virtual
                    | MethodAttributes.Final,
                    facMethod.FactoryInterfaceMethodInfo.ReturnType,
                    types);

                // assign the method parameters
                // into the constructed type ctor
                var methodIL = methodBuilder.GetILGenerator();
                methodIL.Emit(OpCodes.Nop);
                //methodIL.Emit(OpCodes.Nop);
                for (int i = 0; i < facMethod.Parameters.Count; i++)
                {
                    // different op codes if the parameter is derived from field
                    if (facMethod.Parameters[i].IsFromField)
                    {
                        methodIL.Emit(OpCodes.Ldarg_0);
                        methodIL.Emit(OpCodes.Ldfld, fieldBuilders[facMethod.Parameters[i].MatchingField]);
                        continue;
                    }

                    if (facMethod.Parameters[i].IsFactoryReinjection)
                    {
                        methodIL.Emit(OpCodes.Ldarg_0);
                    }
                    else
                    {
                        if (i == 0)
                            methodIL.Emit(OpCodes.Ldarg_1);
                        else if (i == 1)
                            methodIL.Emit(OpCodes.Ldarg_2);
                        else if (i == 2)
                            methodIL.Emit(OpCodes.Ldarg_3);
                        else
                            methodIL.Emit(OpCodes.Ldarg_S, facMethod.Parameters[i].MatchingField);
                    }
                }

                methodIL.Emit(OpCodes.Newobj, facMethod.MatchingCtor);
                methodIL.Emit(OpCodes.Ret);
            }

            return facBuilder.CreateTypeInfo();

        }
    }
}