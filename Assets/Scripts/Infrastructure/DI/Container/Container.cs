using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Infrastructure.DI.Injector;
using Infrastructure.DI.Model;
using Infrastructure.DI.ServiceLocator;
using UnityEngine;
using ServiceDescriptor = Infrastructure.DI.ServiceLocator.ServiceDescriptor;

namespace Infrastructure.DI.Container
{
    public class Container : IContainer
    {
        private class Scope : IScope
        {
            private readonly Container _container;
            private readonly ConcurrentDictionary<Type, object> _scopedInstance = new();

            public Scope(Container container)
            {
                _container = container;
            }

            public object Resolve(Type service)
            {
                var descriptor = _container.FindDescriptors(service);
                if (descriptor.LifeTime == LifeTime.Transient)
                    return _container.CreateInstance(service, this);
                else if (descriptor.LifeTime == LifeTime.Scoped || _container._rootScope == this)
                {
                    return _scopedInstance.GetOrAdd(service, s => _container.CreateInstance(s, this));
                }
                else
                {
                    return _container._rootScope.Resolve(service);
                }
            }
        }

        private readonly ServiceDescriptor _serviceDescriptor;
        private readonly DescriptorDependencyInjector _dependencyInjector;
        private readonly ConcurrentDictionary<Type,Func<IScope, object>> _cachedBuildActivators = new();
        private readonly ServiceLocatorProvider _locatorProvider;
        private readonly GameStateLifetimeManager _gameStateLifeManager;
        
        private readonly Scope _rootScope;

        public Container(IEnumerable<Model.ServiceDescriptor> descriptors, IServiceLocator serviceLocator = null)
        {
            Dictionary<Type, Model.ServiceDescriptor> dictionaryDescriptors = descriptors.ToDictionary(x => x.ServiceType);
            _serviceDescriptor = new ServiceDescriptor(dictionaryDescriptors);
            _locatorProvider = new ServiceLocatorProvider();
            _gameStateLifeManager = new GameStateLifetimeManager();
            _dependencyInjector = new DescriptorDependencyInjector(_serviceDescriptor, _locatorProvider, _gameStateLifeManager, this);
           
            _rootScope = new Scope(this);
        }
        
        public Container(IServiceLocator serviceLocator = null)
        {
            _serviceDescriptor = new ServiceDescriptor();
            _locatorProvider = new ServiceLocatorProvider();
            _gameStateLifeManager = new GameStateLifetimeManager();
            _dependencyInjector = new DescriptorDependencyInjector(_serviceDescriptor, _locatorProvider, _gameStateLifeManager, this);
            
            _rootScope = new Scope(this);
        }

        public static void PrintServiceLocator()
        {
            DictionaryServiceLocator.PrintAllServices();
        }


        public object ReturnInjectArgument(Model.ServiceDescriptor descriptor, Type type)
        {
            object service = descriptor.LifeTime == LifeTime.Transient ? CreateInstance(type, _rootScope) : _rootScope.Resolve(type);
            return service;
        }

        private Model.ServiceDescriptor FindDescriptors(Type service)
        {
           return _serviceDescriptor.TryGetType(service);
        }

        private object CreateInstance(Type service, IScope scope)
        {
            return _cachedBuildActivators.GetOrAdd(service, key => BuildActivation(key))(scope);
        }

        public IScope CreateScope()
        {
            return new Scope(this);
        }

        public void BindData(Type type, Type service, LifeTime lifetime)
        {
            
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                if (service == null)
                {
                    Debug.Log("Класс MonoBehaviour не может быть равен null");
                }
            }

            TypeBasedServiceDescriptor serviceDescriptor = new TypeBasedServiceDescriptor() 
                {ImplementationType = service, ServiceType = type, LifeTime = lifetime };
            _serviceDescriptor.BindData(type,serviceDescriptor);
        }

        public void Construct(object target)
        {
            _dependencyInjector.Inject(target);
        }

        public void CacheType(Type type, object instance)
        {
            _locatorProvider.Default.BindData(type, instance);
            
        }

        public void CacheMono<T>(Type type, T instance) where T : MonoBehaviour
        {
            _locatorProvider.Mono.BindData(type, instance);
        }

        public void CacheComponent<T>(Type componentType, T instance) where T : Component
        {
            _locatorProvider.Component.BindData(componentType, instance);
        }

        public void CacheScriptableObject<T>(Type type, T instance) where T : ScriptableObject
        {
            _locatorProvider.Scriptable.BindData(type, instance);
        }

        public void RegisterTransientWithTimeState(Type type, Type type1, DIGameState diGameState)
        {
            var instance = BuildActivation(type, true)(_rootScope);
            _gameStateLifeManager.Track(DIGameState.BootStateLife, instance);
        }

        private Func<IScope, object> BuildActivation(Type service, bool useDirectType = false)
        {
            Type implementationType;

            if (useDirectType)
            {
                implementationType = service;
            }
            else
            {
                var descriptor = _serviceDescriptor.TryGetType(service);
                if (descriptor is null)
                    throw new InvalidOperationException($"Service {service} is not registered.");

                if (descriptor is InstanceBasedServiceDescriptor ib)
                    return _ => ib.Instance;

                if (descriptor is FactoryBasedServiceDescriptor fb)
                    return _ => fb.Factory;

                implementationType = ((TypeBasedServiceDescriptor)descriptor).ImplementationType;
            }

            var ctor = implementationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
            var parameters = ctor.GetParameters();

            var scopeParam = Expression.Parameter(typeof(IScope), "scope");

            var argsExpressions = new Expression[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;

                var containerExpr = Expression.Constant(this);
                var typeExpr = Expression.Constant(paramType, typeof(Type));
                var callExpr = Expression.Call(containerExpr,
                    typeof(Container).GetMethod("CreateInstance", BindingFlags.NonPublic | BindingFlags.Instance),
                    typeExpr, scopeParam);

                argsExpressions[i] = Expression.Convert(callExpr, paramType);
            }

            var newExpr = Expression.New(ctor, argsExpressions);
            var lambda = Expression.Lambda<Func<IScope, object>>(newExpr, scopeParam);

            return lambda.Compile();
        }
        
        
    }
}