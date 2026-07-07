using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Api.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        //should be configured in secrets 
        private const string DockerConnectionString =
            "Server=localhost,1433;Database=TodoDbTest;User Id=sa;Password=YourStrong@Password1;TrustServerCertificate=True";

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = DockerConnectionString
                });
            });

            return base.CreateHost(builder);
        }
    }
}
