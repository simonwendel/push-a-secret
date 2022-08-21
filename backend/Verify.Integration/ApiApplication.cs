// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using System.Linq;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Storage;

namespace Verify.Integration;

internal class ApiApplication : WebApplicationFactory<Program>
{
    private readonly bool crashOnRequest;

    public ApiApplication(bool crashOnRequest = false)
    {
        this.crashOnRequest = crashOnRequest;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(serviceCollection =>
        {
            var descriptor = serviceCollection.SingleOrDefault(x => x.ServiceType == typeof(IRepository));
            if (descriptor != null)
            {
                serviceCollection.Remove(descriptor);
            }

            if (crashOnRequest)
            {
                serviceCollection.AddTransient<IRepository>(_ => throw new InvalidOperationException());
                return;
            }

            serviceCollection.AddTransient<IMongoDbRepositoryFactory, TestMongoDbRepositoryFactory>();
            serviceCollection.AddTransient<IRepository>(services => services.GetService<IMongoDbRepositoryFactory>()!.Build());
        });
    }
}
