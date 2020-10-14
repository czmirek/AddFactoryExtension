namespace Microsoft.Extensions.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection.AddFactoryExtension;
    using System;
    using System.Reflection;

    public static partial class AddFactoryServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a dynamic implementation of the <typeparamref name="TFactory"/> factory interface
        /// without the need of factory implementation.
        /// </summary>
        /// <typeparam name="TFactory">Interface type of the factory</typeparam>
        /// <param name="sc">Service collection</param>
        /// <param name="serviceLifetime">Lifetime of the factory. Singleton is default.</param>
        /// <param name="scanAssemblies">Assemblies which to scan for types of interface implementations in factory return values.</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddFactory<TFactory>(this IServiceCollection sc, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Assembly[] scanAssemblies) where TFactory : class
        {
            FactoryClassBuilder facClsBuilder = FactoryClassBuilder.CreateFactoryClassBuilder<TFactory>(scanAssemblies);

            sc.Add(new ServiceDescriptor(typeof(TFactory), (sp) =>
            {
                Type facType = ILFactoryTypeCreator.CreateType<TFactory>(facClsBuilder);
                return FactoryTypeActivator.Activate(sp, facType);

            }, serviceLifetime));

            return sc;
        }

        /// <summary>
        /// Adds a dynamic implementation of the <typeparamref name="TFactory"/> factory interface
        /// without the need of factory implementation. This method searches for interface implementations
        /// in the assembly of the factory interface type.
        /// </summary>
        /// <typeparam name="TFactory">Interface type of the factory</typeparam>
        /// <param name="sc">Service collection</param>
        /// <param name="serviceLifetime">Lifetime of the factory. Singleton is default.</param>
        /// <returns></returns>
        public static IServiceCollection AddFactory<TFactory>(this IServiceCollection sc, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton) where TFactory : class
        {
            return AddFactory<TFactory>(sc, serviceLifetime, typeof(TFactory).Assembly);
        }
    }
}
