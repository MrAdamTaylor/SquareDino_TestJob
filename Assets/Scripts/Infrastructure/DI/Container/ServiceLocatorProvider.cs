using Infrastructure.DI.ServiceLocator;

namespace Infrastructure.DI.Container
{
    public class ServiceLocatorProvider
    {
        private readonly IServiceLocator _defaultLocator;
        private readonly IServiceLocator _monoLocator;
        private readonly IServiceLocator _scriptableLocator;
        
        public ServiceLocatorProvider(
            IServiceLocator defaultLocator = null,
            IServiceLocator monoLocator = null,
            IServiceLocator scriptableLocator = null)
        {
            _defaultLocator = defaultLocator ?? new DictionaryServiceLocator();
            _monoLocator = monoLocator ?? new MonoDictionaryServiceLocator();
            _scriptableLocator = scriptableLocator ?? new DictionaryServiceLocator();
        }
        
        public IServiceLocator GetLocator(LocatorType type)
        {
            return type switch
            {
                LocatorType.Mono => _monoLocator,
                LocatorType.Scriptable => _scriptableLocator,
                _ => _defaultLocator,
            };
        }

        public IServiceLocator Default => _defaultLocator;
        public IServiceLocator Mono => _monoLocator;
        public IServiceLocator Scriptable => _scriptableLocator;
        
    }
}