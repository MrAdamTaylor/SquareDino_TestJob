using System;

namespace Infrastructure.DI.Model
{
    public static class ContainerBuilderExtensions
    {
        private static IContainerBuilder RegisterType(this IContainerBuilder builder, Type service, Type implementation,
            LifeTime lifetime)
        {
            builder.Register(new TypeBasedServiceDescriptor() { ImplementationType = implementation, ServiceType = service, LifeTime = lifetime});
            return builder;
        }

        private static IContainerBuilder RegisterFactory(this IContainerBuilder builder, Type service, Func<IScope, object> factory, LifeTime lifetime)
        {
            builder.Register(new FactoryBasedServiceDescriptor() { Factory = factory, ServiceType = service, LifeTime = lifetime});
            return builder;
        }

        private static IContainerBuilder RegisterInstance(this IContainerBuilder builder, Type service, object instance)
        {
            builder.Register(new InstanceBasedServiceDescriptor(service, instance));
            return builder;
        }
    
        public static IContainerBuilder RegisterTransient(this IContainerBuilder builder, Type service, Type implementation)
            => builder.RegisterType(service, implementation, LifeTime.Transient);
    
        public static IContainerBuilder RegisterScoped(this IContainerBuilder builder, Type service, Type implementation)
            => builder.RegisterType(service, implementation, LifeTime.Scoped);
    
        public static IContainerBuilder RegisterSingleton(this IContainerBuilder builder, Type service, Type implementation)
            => builder.RegisterType(service, implementation, LifeTime.Singleton);
    
        public static IContainerBuilder RegisterTransient(this IContainerBuilder builder, Type service, Func<IScope, object> factory)
            => builder.RegisterFactory(service, factory, LifeTime.Transient);
    
        public static IContainerBuilder RegisterScoped(this IContainerBuilder builder, Type service, Func<IScope, object> factory)
            => builder.RegisterFactory(service, factory, LifeTime.Scoped);
    
        public static IContainerBuilder RegisterSingleton(this IContainerBuilder builder, Type service, Func<IScope, object> factory)
            => builder.RegisterFactory(service, factory, LifeTime.Singleton);

        public static IContainerBuilder RegisterSingleton(this IContainerBuilder builder, Type service, object instance)
            => builder.RegisterInstance(service, instance);
    
        public static IContainerBuilder RegisterTransient<TService, TImplementation>(this IContainerBuilder builder) where TImplementation : TService
            => builder.RegisterType(typeof(TService), typeof(TImplementation), LifeTime.Transient);
    
        public static IContainerBuilder RegisterScoped<TService, TImplementation>(this IContainerBuilder builder) where TImplementation : TService
            => builder.RegisterType(typeof(TService), typeof(TImplementation), LifeTime.Scoped);
    
        public static IContainerBuilder RegisterSingleton<TService, TImplementation>(this IContainerBuilder builder) where TImplementation : TService
            => builder.RegisterType(typeof(TService), typeof(TImplementation), LifeTime.Singleton);
    
        public static IContainerBuilder RegisterSingleton<T>(this IContainerBuilder builder, object instance) 
            => builder.RegisterInstance(typeof(T), instance);
    }
}
