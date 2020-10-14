using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace AddFactoryExtension.Tests
{
    /// <summary>
    /// https://stackoverflow.com/questions/64353113/emit-a-factory-method
    /// </summary>
    public class EmitValidILCodeFactoryTest
    {
        public interface IBar { }
        public class Bar : IBar { }
        public interface IBarFactory { IBar Factory(); }

        [Fact]
        public void Verify_IL_Code_Works()
        {
            AssemblyName asmName = new AssemblyName { Name = "ToFactoryDynamicAssembly" };
            AssemblyBuilder asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule("ToFactoryModule");
            ConstructorInfo systemObjectCtor = Type.GetType("System.Object").GetConstructor(new Type[0]);

            TypeBuilder facBuilder = moduleBuilder.DefineType($"DynamicFactoryFor{typeof(IBarFactory).Name}",
                TypeAttributes.Public
                | TypeAttributes.AutoClass
                | TypeAttributes.AnsiClass
                | TypeAttributes.BeforeFieldInit);

            facBuilder.AddInterfaceImplementation(typeof(IBarFactory));

            ConstructorBuilder facCtorBuilder = facBuilder.DefineConstructor(MethodAttributes.Public
                | MethodAttributes.HideBySig
                | MethodAttributes.SpecialName
                | MethodAttributes.RTSpecialName, CallingConventions.Standard, Type.EmptyTypes);

            var ctorIL = facCtorBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, systemObjectCtor);
            ctorIL.Emit(OpCodes.Nop);
            ctorIL.Emit(OpCodes.Ret);

            var methodBuilder = facBuilder.DefineMethod("Factory", 
                MethodAttributes.Public
                | MethodAttributes.HideBySig
                | MethodAttributes.NewSlot
                | MethodAttributes.Virtual
                | MethodAttributes.Final, typeof(IBar), Type.EmptyTypes);

            var BarCtor = typeof(Bar).GetConstructors()[0];
            var methodIL = methodBuilder.GetILGenerator();

            methodIL.Emit(OpCodes.Newobj, BarCtor);
            methodIL.Emit(OpCodes.Ret);

            var type = facBuilder.CreateType();

            var ctor = type.GetConstructors()[0];
            IBarFactory factory = ctor.Invoke(null) as IBarFactory;
            IBar Bar = factory.Factory();

            Assert.NotNull(Bar);
        }
    }
}
