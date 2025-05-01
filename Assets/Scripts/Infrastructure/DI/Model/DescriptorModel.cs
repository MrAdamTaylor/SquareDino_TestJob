using System;

namespace Infrastructure.DI.Model
{
    public enum LifeTime
    {
        Transient,
        Scoped,
        Singleton
    }

    public abstract class ServiceDescriptor
    {
        public Type ServiceType { get; set; }
        public LifeTime LifeTime { get; set; }
    }
    
    public class TypeBasedServiceDescriptor : ServiceDescriptor
    {
        public Type ImplementationType { get; set; }
    }

    public class FactoryBasedServiceDescriptor : ServiceDescriptor
    {
        public Func<IScope, object> Factory { get; set; }
    }

    public class InstanceBasedServiceDescriptor : ServiceDescriptor
    {
        public object Instance { get; set; }

        public InstanceBasedServiceDescriptor(Type serviceType, object instance)
        {
            LifeTime = LifeTime.Singleton;
            ServiceType = serviceType;
            Instance = instance;
        }
    }
}