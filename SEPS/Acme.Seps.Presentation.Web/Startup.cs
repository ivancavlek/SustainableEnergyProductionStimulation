﻿using Acme.Seps.Presentation.Web.DependencyInjection;
using Acme.Seps.Presentation.Web.Filters;
using Acme.Seps.Presentation.Web.Utility;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;

namespace Acme.Seps.Presentation.Web;

public class Startup
{
    private readonly SepsSimpleInjectorContainer _container;

    public Startup() => _container = SepsSimpleInjectorContainer.Container;

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddMvc(cfg => cfg.Filters.Add(new ValidateModelAttribute()))
            .AddFluentValidation(fvn =>
                fvn.ValidatorFactory = new DependencyInjectionFluentValidatorFactory(_container));

        IntegrateSimpleInjector(services);
    }

    private void IntegrateSimpleInjector(IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(_container));
        services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(_container));

        services.EnableSimpleInjectorCrossWiring(_container);
        services.UseSimpleInjectorAspNetRequestScoping(_container);
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        InitializeContainer(app, env);

        app.UseMvc(routes =>
            routes.MapRoute(
                name: "default",
                template: "api/{area}/{controller}/{action}/{id:guid?}"));

    }

    private void InitializeContainer(IApplicationBuilder app, IHostingEnvironment env)
    {
        _container.RegisterMvcControllers(app);
        _container.RegisterMvcViewComponents(app);

        if (env.IsEnvironment("IntegrationTesting"))
            _container.RegisterForTest();

        _container.AutoCrossWireAspNetComponents(app);
    }
}