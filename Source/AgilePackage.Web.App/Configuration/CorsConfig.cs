using Microsoft.Extensions.DependencyInjection;

namespace AgilePackage.Web.App
{
    public static class CorsConfig
    {
        public static string CorsName { get => "AgilePackageCors"; }

        public static IServiceCollection AddCorsConfig(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: CorsName,
                                  builder =>
                                  {
                                      builder.AllowAnyHeader().AllowAnyMethod();
                                  });
            });

            return services;
        }
    }
}
