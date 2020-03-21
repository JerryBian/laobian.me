﻿using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Laobian.Blog.Helpers;
using Laobian.Share;
using Laobian.Share.Blog.Alert;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;

namespace Laobian.Blog
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Global.Environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            StartupHelper.RegisterService(services, Configuration);

            var builder = services.AddControllersWithViews();
            if (Global.Environment.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }

            services.AddDirectoryBrowser();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("zh-cn");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("zh-cn");

            var alertService = app.ApplicationServices.GetService<IBlogAlertService>();
            var logger = app.ApplicationServices.GetService<ILogger<Startup>>();

            RegisterEvents(applicationLifetime, alertService, logger);
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            if (Global.Environment.IsProduction())
            {
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandler = async context =>
                    {
                        logger.LogError(context.Features.Get<IExceptionHandlerFeature>()?.Error,
                            $"Something is wrong! Request Url= {context.Request.Path}");
                        await context.Response.WriteAsync(
                            $"Something was wrong! Please contact {Global.Config.Common.AdminEmail}.");
                    }
                });
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                using (LogContext.PushProperty(LogEntryItem.RequestUrl.ToString(), context.Request.GetDisplayUrl()))
                using (LogContext.PushProperty(LogEntryItem.RequestIp.ToString(), context.Connection.RemoteIpAddress))
                using (LogContext.PushProperty(LogEntryItem.RequestUserAgent.ToString(), context.Request.Headers["User-Agent"]))
                {
                    await next();
                }
            });

            app.UseSerilogRequestLogging();

            app.UseStatusCodePages(async context =>
            {
                var message = "Status code page, status code: " +
                              context.HttpContext.Response.StatusCode;
                logger.LogWarning(message);
                context.HttpContext.Response.ContentType = "text/plain";
                await context.HttpContext.Response.WriteAsync(message);
            });

            app.UseStaticFiles();

            var fileDirFullPath = Path.Combine(Global.Config.Blog.AssetRepoLocalDir, Global.Config.Blog.FileGitPath);
            Directory.CreateDirectory(fileDirFullPath);
            var fileServerOptions = new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(fileDirFullPath),
                RequestPath = Global.Config.Blog.FileRequestPath,
                EnableDirectoryBrowsing = true
            };
            app.UseFileServer(fileServerOptions);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        }

        private static void RegisterEvents(
            IHostApplicationLifetime applicationLifetime,
            IBlogAlertService alertService,
            ILogger<Startup> logger)
        {
            applicationLifetime.ApplicationStarted.Register(async () =>
            {
                Global.StartTime = DateTime.Now;
                var appVersion = Assembly.GetEntryAssembly()?.GetName().Version;
                Global.AppVersion = appVersion == null ? "1.0" : $"{appVersion.Major}.{appVersion.Minor}";

                if (!Global.Environment.IsDevelopment())
                {
                    await alertService.AlertEventAsync("Blog started.");
                }

                logger.LogInformation("Application started.");
            });

            applicationLifetime.ApplicationStopping.Register(async () =>
            {
                if (!Global.Environment.IsDevelopment())
                {
                    await alertService.AlertEventAsync("Blog is stopping.");
                }

                logger.LogInformation("Application is stopping.");
            });
        }
    }
}