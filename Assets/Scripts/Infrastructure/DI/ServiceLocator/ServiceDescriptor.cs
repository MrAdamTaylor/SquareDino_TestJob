using System;
using System.Collections.Generic;

namespace Infrastructure.DI.ServiceLocator
{
    public class ServiceDescriptor 
    {
        private readonly Dictionary<Type, Model.ServiceDescriptor> _descriptors;

        public ServiceDescriptor(Dictionary<Type, Model.ServiceDescriptor> descriptors)
        {
            _descriptors = descriptors;
        }
        
        public ServiceDescriptor()
        {
            _descriptors = new Dictionary<Type, Model.ServiceDescriptor>();
        }

        public void BindData(Type type, object obj)
        {
            _descriptors.Add(type,(Model.ServiceDescriptor)obj);
        }

        public object GetData(Type type)
        {
            return _descriptors[type];
        }

        public Model.ServiceDescriptor GetDescriptor(Type type)
        {
            return _descriptors[type];
        }

        public Model.ServiceDescriptor TryGetType(Type service)
        {
            _descriptors.TryGetValue(service, out var result);
            return result;
        }
    }
}
