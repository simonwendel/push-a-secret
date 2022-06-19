using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class IoC
{
    public static IServiceCollection AddDomainModule(this IServiceCollection serviceCollection)
        => serviceCollection
            .AddTransient<ITimestampGenerator, TimestampGenerator>()
            .AddTransient<IIdGenerator, IdGenerator>()
            .AddTransient<IStore, Store>()
            .AddTransient<IBaseConverter, BaseConverter>();
}
