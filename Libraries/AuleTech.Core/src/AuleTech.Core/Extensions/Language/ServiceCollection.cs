using Microsoft.Extensions.DependencyInjection;

namespace AuleTech.Core.Extensions.Language;

public static class ServiceCollection
{
    extension(IServiceCollection serviceCollection)
    {
        public bool IsRegistered<TService>() => serviceCollection.IsRegistered(typeof(TService));
        public bool IsRegistered(Type serviceType) => serviceCollection.Any(x => x.ServiceType == serviceType);
    }
}
