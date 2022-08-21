// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

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
