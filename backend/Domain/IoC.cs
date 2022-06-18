using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class IoC
{
    public static IServiceCollection AddDomainModule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ITimestampGenerator, TimestampGenerator>();
        serviceCollection.AddTransient<IIdGenerator, IdGenerator>();
        serviceCollection.AddTransient<IStore, Store>();
        serviceCollection.AddTransient<IBaseConverter, BaseConverter>();
        return serviceCollection;
    }
}
