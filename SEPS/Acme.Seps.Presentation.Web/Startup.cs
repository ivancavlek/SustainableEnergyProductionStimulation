using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Repository.Base;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Parameter.CommandValidation;
using Acme.Seps.Presentation.Web.Filters;
using Acme.Seps.Repository.Parameter;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
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
        private readonly Container _container;

        public Startup()
        {
            _container = new Container();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(cfg => cfg.Filters.Add(new ValidateModelAttribute()))
                .AddFluentValidation(fvn => fvn.ValidatorFactory = new SimpleInjectorFluentValidatorFactory(_container));

            IntegrateSimpleInjector(services);
        }

        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            _container.Options.DefaultLifestyle = new AsyncScopedLifestyle();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(_container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(_container));

            services.EnableSimpleInjectorCrossWiring(_container);
            services.UseSimpleInjectorAspNetRequestScoping(_container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitializeContainer(app, env);

            //_container.Verify(); doesn't verify due to FluentValidator

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
            _container.RegisterMvcControllers(app);
            _container.RegisterMvcViewComponents(app);

            SetContainerApplicationRegistrations(env);

            // Allow Simple Injector to resolve services from ASP.NET Core.
            _container.AutoCrossWireAspNetComponents(app);
        }

        private void SetContainerApplicationRegistrations(IHostingEnvironment env)
        {
            var onDot = ".".ToCharArray();
            var currentAssemblyPartedFullName = Assembly.GetExecutingAssembly().GetName().Name.Split(onDot);

            var projectName = currentAssemblyPartedFullName[0];
            var currentProjectName = currentAssemblyPartedFullName.Last();

            _container.Register(typeof(IValidator<>), typeof(CalculateCpiCommandValidator).Assembly);
            //_container.Register(typeof(IRuleBuilder<,>), typeof(CalculateCpiCommandValidator).Assembly);

            DbContextOptionsBuilder<BaseContext> options = new DbContextOptionsBuilder<BaseContext>();

            if (env.IsEnvironment("IntegrationTesting"))
            {
                //options.UseSqlite(new SqliteConnection("DataSource=:memory:"));
                options.UseSqlServer(@"Server=DL006132\IVAN;Database=IntegrationTesting;Trusted_Connection=True;");
                var bla = new ParameterContext(options.Options);
                bla.Database.EnsureCreated();
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
                !type.GetInterfaces().Any(ite => ite == typeof(IPeriodFactory)) &&
                !type.GetInterfaces().Any(ite => ite == typeof(IAggregateRoot));

            (List<Type> Abstractions, Type Implementation) InterfaceAbstractionsWithImplementation(Type type) =>
                (type.GetInterfaces().Where(ite => ite.Namespace.Split(onDot).First().Equals(projectName)).ToList(),
                type);

            void RegisterAbstractionsWithImplementation((List<Type> Abstractions, Type Implementation) registration) =>
                registration.Abstractions.ForEach(asn =>
                {
                    switch (registration.Implementation.Name)
                    {
                        case nameof(GuidIdentityFactory):
                            _container.Register(asn, () => new GuidIdentityFactory(SequentialGuidType.SequentialAtEnd));
                            break;
                        case nameof(ParameterContext):
                            _container.Register(asn, () => new ParameterContext(options.Options));
                            break;
                        default:
                            _container.Register(asn, registration.Implementation);
                            break;
                    }
                });
        }
    }
}