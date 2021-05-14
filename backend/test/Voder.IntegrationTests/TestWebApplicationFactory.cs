using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Voder.Models;

namespace Voder.IntegrationTests
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<VoderContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<VoderContext>((options, context) =>
                {
                    context.UseInMemoryDatabase("VoderTest");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<VoderContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<TestWebApplicationFactory<TStartup>>>();

                    db.Database.EnsureCreated();

                    try
                    {
                        SeedData.PopulateTestData(db);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(
                            CheckoutException.Canceled,
                            "An error occured seeding the database #{Message}",
                            e.Message
                        );
                    }
                }
            });
        }
    }
}
