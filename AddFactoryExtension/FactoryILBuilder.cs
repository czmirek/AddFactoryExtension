using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Microsoft.Extensions.DependencyInjection.AddFactoryExtension
{
    internal static class FactoryILBuilder
    {
        public static Type CreateType<TFactory>(FactoryClassBuilder builder) where TFactory : class
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            AppDomain appDomain = Thread.GetDomain();
            AssemblyName asmName = new AssemblyName { Name = "ToFactoryDynamicAssembly" };
            AssemblyBuilder asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule("ToFactoryModule");
            Type objType = Type.GetType("System.Object");
            ConstructorInfo objCtor = objType.GetConstructor(new Type[0]);

            TypeBuilder facBuilder = moduleBuilder.DefineType($"DynamicFactoryFor{typeof(TFactory).Name}");

            var ctorParams = builder.PrivateReadonlyFields.Select(t => t.Type).ToArray();
            ConstructorBuilder facCtorBuilder = facBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctorParams);
            var ctorIL = facCtorBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, objCtor);
            ctorIL.Emit(OpCodes.Nop);
            ctorIL.Emit(OpCodes.Nop);

            Dictionary<string, FieldBuilder> fieldBuilders = new Dictionary<string, FieldBuilder>();
            for (int i = 0; i < builder.PrivateReadonlyFields.Count; i++)
            {
                FieldBuilder fieldBuilder = facBuilder.DefineField
                (
                    builder.PrivateReadonlyFields[i].Name,
                    builder.PrivateReadonlyFields[i].Type,
                    FieldAttributes.Public | FieldAttributes.InitOnly
                );

                ctorIL.Emit(OpCodes.Ldarg_0);

                if (i == 0)
                    ctorIL.Emit(OpCodes.Ldarg_1);
                else if (i == 1)
                    ctorIL.Emit(OpCodes.Ldarg_2);
                else if (i == 2)
                    ctorIL.Emit(OpCodes.Ldarg_3);
                else
                    ctorIL.Emit(OpCodes.Ldarg_S, builder.PrivateReadonlyFields[i].Name);

                ctorIL.Emit(OpCodes.Stfld, fieldBuilder);

                fieldBuilders.Add(builder.PrivateReadonlyFields[i].Name, fieldBuilder);
            }

            ctorIL.Emit(OpCodes.Ret);

            foreach (var facMethod in builder.FactoryMethods)
            {
                var types = facMethod.Parameters.Where(p => !p.IsFromField).Select(p => p.Type).ToArray();
                var methodBuilder = facBuilder.DefineMethod("Factory", MethodAttributes.Public,
                    facMethod.FactoryInterfaceMethodInfo.ReturnType,
                    types);

                var methodIL = methodBuilder.GetILGenerator();
                methodIL.Emit(OpCodes.Nop);
                for (int i = 0; i < facMethod.Parameters.Count; i++)
                {
                    if (i == 0)
                        methodIL.Emit(OpCodes.Ldarg_1);
                    else if (i == 1)
                        methodIL.Emit(OpCodes.Ldarg_2);
                    else if (i == 2)
                        methodIL.Emit(OpCodes.Ldarg_3);
                    else
                        methodIL.Emit(OpCodes.Ldarg_S, facMethod.Parameters[i].MatchingField);

                    if (facMethod.Parameters[i].IsFromField)
                    {
                        methodIL.Emit(OpCodes.Ldarg_0);
                        methodIL.Emit(OpCodes.Ldfld, fieldBuilders[facMethod.Parameters[i].MatchingField]);
                    }
                }

                methodIL.Emit(OpCodes.Stloc_0);
                methodIL.Emit(OpCodes.Ldloc_0);
                methodIL.Emit(OpCodes.Ret);
            }

            return facBuilder.CreateTypeInfo();

        }
    }
}