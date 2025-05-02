using Infrastructure.DI.ServiceLocator;

namespace Infrastructure.DI.Container
{
    public class ServiceLocatorProvider
    {
        private readonly IServiceLocator _defaultLocator;
        private readonly IServiceLocator _monoLocator;
        private readonly IServiceLocator _scriptableLocator;
        private readonly IServiceLocator _componentsLocator;
        
        public ServiceLocatorProvider(
            IServiceLocator defaultLocator = null,
            IServiceLocator monoLocator = null,
            IServiceLocator scriptableLocator = null,
            IServiceLocator componentLocator = null)
        {
            _defaultLocator = defaultLocator ?? new DictionaryServiceLocator();
            _monoLocator = monoLocator ?? new MonoDictionaryServiceLocator();
            _scriptableLocator = scriptableLocator ?? new DictionaryServiceLocator();
            _componentsLocator = componentLocator ?? new ComponentDictionaryServiceLocator();
        }
        
        public IServiceLocator GetLocator(LocatorType type)
        {
            return type switch
            {
                LocatorType.Mono => _monoLocator,
                LocatorType.Scriptable => _scriptableLocator,
                LocatorType.Component => _componentsLocator,
                _ => _defaultLocator,
            };
        }

        public IServiceLocator Default => _defaultLocator;
        public IServiceLocator Mono => _monoLocator;
        public IServiceLocator Scriptable => _scriptableLocator;
        public IServiceLocator Component => _componentsLocator;
        
    }
}