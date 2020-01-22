using ExerciseTracker.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExerciseTracker.Fixtures.Factories
{
    public class InMemoryApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class    {
        public SqliteConnection Connection { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseEnvironment("Testing")
                //.UseSolutionRelativeContentRoot("")
                // overriding the starup classes' services
                .ConfigureTestServices(services =>
                {
                    Connection = new SqliteConnection("DataSource=:memory:");
                    Connection.Open();
                    var options = new DbContextOptionsBuilder<AppDbContext>()
                        .UseSqlite(Connection)
                        .Options;

                    // register the test DB Context in the test service container
                    services.AddScoped<AppDbContext>(_ => new TestDbContext(options));

                    // Build an intermediate service provider
                    // allows user to extract registered dependencies from the container up 
                    // to this point.
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();

                    // get the registed TestDbContext
                    using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    dbContext.Database.EnsureCreated();
                });
        }

        protected override void Dispose(bool disposing)
        {
            Connection.Close();
            base.Dispose(disposing);
        }
    }
}
