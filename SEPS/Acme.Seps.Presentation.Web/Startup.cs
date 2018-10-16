using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Repository.Base;
using Acme.Seps.Repository.Parameter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acme.Seps.Presentation.Web
{
    public class Startup
    {
        private readonly Container container;

        public Startup()
        {
            container = new Container();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            IntegrateSimpleInjector(services);
        }

        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            container.Options.DefaultLifestyle = new AsyncScopedLifestyle();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(container));

            services.EnableSimpleInjectorCrossWiring(container);
            services.UseSimpleInjectorAspNetRequestScoping(container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitializeContainer(app, env);

            container.Verify();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        private void InitializeContainer(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Add application presentation components:
            container.RegisterMvcControllers(app);
            container.RegisterMvcViewComponents(app);

            SetContainerApplicationRegistrations(env);

            // Allow Simple Injector to resolve services from ASP.NET Core.
            container.AutoCrossWireAspNetComponents(app);
        }

        private void SetContainerApplicationRegistrations(IHostingEnvironment env)
        {
            var onDot = ".".ToCharArray();
            var currentAssemblyPartedFullName = Assembly.GetExecutingAssembly().FullName.Split(onDot);

            var projectName = currentAssemblyPartedFullName[0];
            var currentProjectName = currentAssemblyPartedFullName.Last();

            container.Register<IIdentityFactory<Guid>>(() =>
                new GuidIdentityFactory(SequentialGuidType.SequentialAtEnd));

            DbContextOptionsBuilder<BaseContext> options = new DbContextOptionsBuilder<BaseContext>();

            if (env.IsEnvironment("IntegrationTesting"))
            {
                options.UseSqlite(new SqliteConnection("DataSource=:memory:"));
                //container.Register<IUnitOfWork>(() => new ParameterContext(options.Options));
            }

            DependencyContext.Default.RuntimeLibraries
                .Where(RuntimeLibraryIsFromProject)
                .Select(AssemblyFromRuntimeLibrary)
                .SelectMany(TypesFromAssembly)
                .Where(TypeIsForInjection)
                .Select(InterfaceAbstractionsWithImplementation)
                .ToList() // riješiti nekako, možda i dva contexta, jer ovako ili onako ne radim tracked nego not tracked
                .ForEach(RegisterAbstractionsWithImplementation);

            bool RuntimeLibraryIsFromProject(RuntimeLibrary runtimeLibrary) =>
                runtimeLibrary.Name.Contains(projectName);

            Assembly AssemblyFromRuntimeLibrary(RuntimeLibrary runtimeLibrary) =>
                Assembly.Load(new AssemblyName(runtimeLibrary.Name));

            IEnumerable<Type> TypesFromAssembly(Assembly assembly) =>
                assembly.GetExportedTypes();

            bool TypeIsForInjection(Type type) =>
                type.Namespace.Split(onDot).First().Equals(projectName) &&
                !type.Namespace.Split(onDot).Last().Equals(currentProjectName) &&
                type.GetInterfaces().Any(ite => ite.Namespace.Split(onDot).First().Equals(projectName)) &&
                !type.IsAbstract &&
                !type.GetInterfaces().Any(ite => ite == typeof(IIdentityFactory<Guid>)) &&
                !type.GetInterfaces().Any(ite => ite == typeof(IAggregateRoot));

            (List<Type> Abstractions, Type Implementation) InterfaceAbstractionsWithImplementation(Type type) =>
                (type.GetInterfaces().Where(ite => ite.Namespace.Split(onDot).First().Equals(projectName)).ToList(),
                type);

            void RegisterAbstractionsWithImplementation((List<Type> Abstractions, Type Implementation) registration) =>
                registration.Abstractions.ForEach(asn =>
                {
                    if (registration.Implementation == typeof(ParameterContext))
                        container.Register(asn, () => new ParameterContext(options.Options));
                    else
                        container.Register(asn, registration.Implementation);
                });
        }
    }
}