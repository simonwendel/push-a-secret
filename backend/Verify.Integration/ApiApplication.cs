using System.Linq;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Verify.Integration;

internal class ApiApplication : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(IRepository));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddSingleton<IRepository>(_ => TestMongoDbRepositoryFactory.Build());
        });
    }
}
