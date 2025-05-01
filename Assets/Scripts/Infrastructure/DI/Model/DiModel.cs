using System;
using UnityEngine;

namespace Infrastructure.DI.Model
{
    
        public interface IContainerBuilder
        {
            public void Register(ServiceDescriptor descriptor);

            public IContainer Build();
        }
        public interface IContainer
        {
            public IScope CreateScope();

            public void BindData(Type type, Type service, LifeTime lifeTime);
            
            void Construct(object behaviour);
            void CacheType(Type type, object instance);
            void CacheMono<T>(Type type, T instance) where T : MonoBehaviour;
            void RegisterTransientWithTimeState(Type type, Type type1, DIGameState diGameState);
        }
    
        public interface IScope 
        {
            public object Resolve(Type service);
        }
    
}
