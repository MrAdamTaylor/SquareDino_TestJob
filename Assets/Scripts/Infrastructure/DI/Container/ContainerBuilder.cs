using System.Collections.Generic;
using Infrastructure.DI.Model;

namespace Infrastructure.DI.Container
{
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly List<Model.ServiceDescriptor> _descriptors = new();
        
        public void Register(Model.ServiceDescriptor descriptor)
        {
            _descriptors.Add(descriptor);
        }

        public IContainer Build()
        {
            return new Container(_descriptors);
        }
    }
}