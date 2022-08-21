// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Storage;

public static class IoC
{
    public static IServiceCollection AddStorageModule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IMongoDbRepositoryFactory, MongoDbRepositoryFactory>();
        serviceCollection.AddTransient<IRepository>(services => services.GetService<IMongoDbRepositoryFactory>()!.Build());
        return serviceCollection;
    }
}

