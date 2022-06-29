using Microsoft.Extensions.DependencyInjection;
using Validation.Specific;

namespace Validation;

public static class IoC
{
    public static IServiceCollection AddValidationModule(this IServiceCollection serviceCollection)
        => serviceCollection
            .AddTransient<IIdentifierValidator, IdentifierValidator>()
            .AddTransient<ICiphertextValidator, CiphertextValidator>()
            .AddTransient<IAlgorithmValidator, AlgorithmValidator>()
            .AddTransient<IIvValidator, IvValidator>()
            .AddTransient<ISecretValidator, SecretValidator>();
}
