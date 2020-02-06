using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using Microsoft.Extensions.DependencyInjection;
using GithubLantern.Models;
using Microsoft.Extensions.Configuration;

namespace GithubLantern
{
    public class Startup
    {

        /// <summary>
        /// The member used to read the content inside appsettings.json
        /// </summary>
        public IConfiguration _configuration { get; }

        /// <summary>
        /// A constructor help reading the file appsettings.json at startup time.
        /// </summary>
        /// <param name="pConfiguration"></param>
        public Startup(IConfiguration pConfiguration) => _configuration = pConfiguration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<WebHooks>(_configuration.GetSection("WebHooks"));

            services.AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddGitHubWebHooks();
                
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            // app.UseHttpsRedirection();
            app.UseMvc(routes =>
                {routes.MapRoute(
                    name: "default",
                    template: "{controller=GitHub}/{action=Index}");
                }
            );
        }
    }
}
