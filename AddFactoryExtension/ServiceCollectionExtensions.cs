namespace Microsoft.Extensions.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection.AddFactoryExtension;
    using System;
    using System.Reflection;

    public static partial class AddFactoryServiceCollectionExtensions
    {
        public static IServiceCollection AddFactory<TFactory>(this IServiceCollection sc, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Assembly[] scanAssemblies) where TFactory : class
        {
            FactoryClassBuilder facClsBuilder = FactoryClassBuilder.CreateFactoryClassBuilder<TFactory>(scanAssemblies);

            sc.Add(new ServiceDescriptor(typeof(TFactory), (sp) =>
            {
                Type facType = FactoryILBuilder.CreateType<TFactory>(facClsBuilder);
                return FactoryTypeActivator.Activate(sp, facType);

            }, serviceLifetime));

            return sc;
        }



        public static IServiceCollection AddFactory<TFactory>(this IServiceCollection sc, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton) where TFactory : class
        {
            return AddFactory<TFactory>(sc, serviceLifetime, typeof(TFactory).Assembly);
        }
    }
}
