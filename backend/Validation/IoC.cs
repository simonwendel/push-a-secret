using Microsoft.Extensions.DependencyInjection;

namespace Validation;

public static class IoC
{
    public static IServiceCollection AddValidationModule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IIdentifierValidator, IdentifierValidator>();
        serviceCollection.AddTransient<ICiphertextValidator, CiphertextValidator>();
        return serviceCollection;
    }
}
