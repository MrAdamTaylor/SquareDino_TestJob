using System;
using System.Reflection;
using Infrastructure.DI.Container;
using Infrastructure.DI.ServiceLocator;
using UnityEngine;

namespace Infrastructure.DI.Injector
{
    public class DescriptorDependencyInjector : IInjector
    {
        private ServiceDescriptor _serviceDescriptor;
        private ServiceLocatorProvider _locatorProvider;
        private  GameStateLifetimeManager _gameStateLifeManager;
        private Container.Container _container;

        public DescriptorDependencyInjector(ServiceDescriptor service, ServiceLocatorProvider provider,  GameStateLifetimeManager lifeManager,Container.Container container)
        {
            _locatorProvider = provider;
            _gameStateLifeManager = lifeManager;
            _serviceDescriptor = service;
            _container = container;
        }
        
        public void Inject(object target)
        {
            Type type = target.GetType();
            
            MethodInfo[] methods = type.GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.FlattenHierarchy
            );

            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];
                if (method.IsDefined(typeof(InjectAttribute), inherit: true))
                {
                    InvokeConstruct(method, target);
                }
            }
            
            FieldInfo[] fields = type.GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.FlattenHierarchy
            );

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                if (field.IsDefined(typeof(InjectAttribute), inherit: true))
                {
                    InvokeField(field, target);
                }
            }
            
            PropertyInfo[] properties = type.GetProperties(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.FlattenHierarchy
            );

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                if (!property.CanWrite || property.GetSetMethod(true) == null)
                    continue;

                if (property.IsDefined(typeof(InjectAttribute), inherit: true))
                {
                    InvokeProperty(property, target);
                }
            }
        }

        private void InvokeField(FieldInfo field, object target)
        {
            var type = field.FieldType;
            var value = ResolveService(type);
            field.SetValue(target, value);
        }

        private void InvokeProperty(PropertyInfo property, object target)
        {
            if (!property.CanWrite || property.GetSetMethod(true) == null)
                return;

            var type = property.PropertyType;
            var value = ResolveService(type);
            property.SetValue(target, value);
        }

        private void InvokeConstruct(MethodInfo method, object target)
        {
            ParameterInfo[] parameters = method.GetParameters();
            int count = parameters.Length;
            object[] args = new object[count];

            for (int i = 0; i < count; i++)
            {
                Type type = parameters[i].ParameterType;
                args[i] = ResolveService(type);
            }

            method.Invoke(target, args);
        }
        
        private object ResolveService(Type type)
        {
            object service = null;

            if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                if (_locatorProvider.Mono.IsGetData(type))
                {
                    service = _locatorProvider.Mono.GetData(type);
                }
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(type))
            {
                if (_locatorProvider.Scriptable.IsGetData(type))
                {
                    service = _locatorProvider.Scriptable.GetData(type);
                }
            }
            else
            {
                if (_locatorProvider.Default.IsGetData(type))
                {
                    service = _locatorProvider.Default.GetData(type);
                }
                else if (_gameStateLifeManager.IsGetData(type))
                {
                    service = _gameStateLifeManager.ReturnService(type);
                }
            }

            if (service == null)
            {
                var descriptor = _serviceDescriptor.GetDescriptor(type);
                service = _container.ReturnInjectArgument(descriptor, type);
            }

            return service;
        }
    }
}