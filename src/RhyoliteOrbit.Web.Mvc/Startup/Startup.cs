using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Castle.Logging.Log4Net;
using RhyoliteOrbit.Authentication.JwtBearer;
using RhyoliteOrbit.Configuration;
using RhyoliteOrbit.Identity;
using RhyoliteOrbit.Web.Resources;
using Castle.Facilities.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Akka.Actor;
using Akka.Routing;
using HashidsNet;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using RhyoliteOrbit.Actors;
using StackExchange.Redis;

namespace RhyoliteOrbit.Web.Startup;

public class Startup
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IConfigurationRoot _appConfiguration;
    private readonly IConfiguration _configuration;

    public Startup(IWebHostEnvironment env, IConfiguration configuration)
    {
        _hostingEnvironment = env;
        _appConfiguration = env.GetAppConfiguration();
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // MVC
        services.AddControllersWithViews(
                options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
                }
            );

        IdentityRegistrar.Register(services);
        AuthConfigurer.Configure(services, _appConfiguration);

        services.Configure<WebEncoderOptions>(options =>
        {
            options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
        });
        
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
        });
        
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true; // Enable compression for HTTPS requests (optional)
            options.Providers.Add<GzipCompressionProvider>();

        });
        

        services.AddScoped<IWebResourceManager, WebResourceManager>();
        
        services.AddSingleton<IHashids>(_ => new Hashids("13a51c1b296346b5881b69d439749334", 10));
        services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(_configuration.GetValue<string>("RedisCache:ConnectionString")));

        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
        });

        services.AddSignalR();
        
        // Register ActorSystem ...
        services.AddSingleton(_ => ActorSystem.Create($"ActorSystem", ConfigurationLoader.Load()));

        services.AddSingleton<TopLevelActors.EmailActorProvider>(provider =>
        {
            var actorSystem = provider.GetService<ActorSystem>();
            var configuration = provider.GetService<IConfiguration>();
            var emailActor = actorSystem.ActorOf(Props.Create(() => new EmailActor(provider, configuration)).WithRouter(new RoundRobinPool(1)));
            return () => emailActor;
        });
        
        services.AddSingleton<TopLevelActors.StudentBillActorProvider>(provider =>
        {
            var actorSystem = provider.GetService<ActorSystem>();
            var configuration = provider.GetService<IConfiguration>();
            var studentBillActor = actorSystem.ActorOf(Props.Create(() => new StudentBillActor(provider)).WithRouter(new RoundRobinPool(2)));
            return () => studentBillActor;
        });
        
        
        // Configure Abp and Dependency Injection
        services.AddAbpWithoutCreatingServiceProvider<RhyoliteOrbitWebMvcModule>(
            // Configure Log4Net logging
            options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseAbpLog4Net().WithConfig(
                    _hostingEnvironment.IsDevelopment()
                        ? "log4net.config"
                        : "log4net.Production.config"
                    )
            )
        );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        app.UseAbp(); // Initializes ABP framework.

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();

        app.UseJwtTokenMiddleware();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<AbpCommonHub>("/signalr");
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
        });
    }
}
