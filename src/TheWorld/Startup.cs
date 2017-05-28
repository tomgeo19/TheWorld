﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheWorld.Services;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using TheWorld.Models;

namespace TheWorld
{
    public class Startup
    {
        private IHostingEnvironment _ent;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _ent = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(_ent.ContentRootPath)
                .AddJsonFile("config.json");
            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);
            if (_ent.IsEnvironment("Development") || _ent.IsEnvironment("Testing"))
            {
                services.AddScoped<IMailService, DebugMailService>();
                
            }
            else
            {

            }
            services.AddDbContext<WorldContext>();
            services.AddScoped<IWorldRepository, WorldRepository>();
            services.AddTransient<WorldContextSeedData>();

            services.AddLogging();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env, ILoggerFactory loggerFactory,
            WorldContextSeedData seeder)
        {

            loggerFactory.AddConsole();
            loggerFactory.AddDebug(LogLevel.Information);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddDebug(LogLevel.Information);
               // app.UseStatusCodePages();
            }
            else
            {
                loggerFactory.AddDebug(LogLevel.Error);
            }
             //    app.UseDefaultFiles();

            app.UseStaticFiles();
            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "App", action = "Index" }
                    );
            });

            seeder.EnsureSeedData().Wait();
        }
    }
}
