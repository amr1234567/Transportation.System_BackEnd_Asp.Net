namespace Transportation.API.Helpers.DI
{
    public static class ApplicationDi
    {
        public static IServiceCollection AddApplicationDi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddContextDi(configuration);
            services.AddServicesDi();
            return services;
        }
    }
}
