// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

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
            .AddTransient<ITtlDaysValidator, TtlDaysValidator>()
            .AddTransient<ISecretValidator, SecretValidator>()
            .AddTransient<IValidator, Validator>();
}
