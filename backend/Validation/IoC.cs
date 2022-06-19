﻿using Microsoft.Extensions.DependencyInjection;

namespace Validation;

public static class IoC
{
    public static IServiceCollection AddValidationModule(this IServiceCollection serviceCollection)
        => serviceCollection
            .AddTransient<IIdentifierValidator, IdentifierValidator>()
            .AddTransient<ICiphertextValidator, CiphertextValidator>()
            .AddTransient<IAlgorithmValidator, AlgorithmValidator>()
            .AddTransient<IIvValidator, IvValidator>();
}
