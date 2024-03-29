﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Acme.Seps.Presentation.Web.Test.Integration.TestUtility;

public class IntegrationTestingWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTesting"); // set as some sort of global variable between assemblies, no magic strings
        base.ConfigureWebHost(builder);
    }
}