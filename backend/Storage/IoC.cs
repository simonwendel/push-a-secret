using Microsoft.Extensions.DependencyInjection;

namespace Storage;

public static class IoC
{
    public static IServiceCollection AddStorageModule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ITimestampGenerator, TimestampGenerator>();
        serviceCollection.AddTransient<IIdGenerator, IdGenerator>();
        serviceCollection.AddTransient<IRepository>(_ => MongoDbRepositoryFactory.Build());
        return serviceCollection;
    }
}
