using Microsoft.Extensions.DependencyInjection;

namespace Conversion;

public static class IoC
{
    public static IServiceCollection AddConversionModule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IBaseConverter, BaseConverter>();
        return serviceCollection;
    }
}
