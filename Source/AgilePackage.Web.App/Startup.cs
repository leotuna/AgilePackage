using AgilePackage.Core.Services;
using AgilePackage.Web.App.Data;
using AgilePackage.Web.App.Hubs;
using AgilePackage.Web.App.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace AgilePackage.Web.App
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();

            services.AddDbContext<AgilePackageDbContext>();

            services.AddScoped<EmailService>();

            services.AddScoped<UserBelongsToProjectService>();

            services.AddSingleton<LiveRetrospective>();

            services.AddSingleton<List<RoomHubMember>>();

            services.AddSignalR();

            services.AddIdentityConfig();

            services.AddCorsConfig();

            services.AddWebOptimizer();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseCors(CorsConfig.CorsName);

            app.UseResponseCompression();

            app.UseHttpsRedirection();

            app.UseWebOptimizer();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<RoomHub>("/signalr/poker");
                endpoints.MapHub<RetrospectivePostVoteHub>("/signalr/retrospective");
                endpoints.MapControllers();
            });
        }
    }
}
