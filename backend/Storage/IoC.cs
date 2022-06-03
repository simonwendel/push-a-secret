using Microsoft.Extensions.DependencyInjection;

namespace Storage;

public static class IoC
{
    public static IServiceCollection AddSecretsStorage(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ITimestampGenerator, TimestampGenerator>();
        serviceCollection.AddTransient<IBase36Converter, Base36Converter>();
        serviceCollection.AddTransient<IIdGenerator, IdGenerator>();
        serviceCollection.AddTransient<IRepository>(_ => MongoDbRepositoryFactory.Build());
        return serviceCollection;
    }
}
