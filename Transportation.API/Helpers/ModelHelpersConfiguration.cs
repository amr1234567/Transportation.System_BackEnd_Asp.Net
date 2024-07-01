using Transportation.Core.Helpers.Classes;

namespace Transportation.API.Helpers
{
    public static class ModelHelpersConfiguration
    {
        public static IServiceCollection AddModelHelpersServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtHelper>(configuration.GetSection("JWT"));
            services.Configure<MailConfigurations>(configuration.GetSection("EmailConfigration"));
            services.Configure<TwilioConfiguration>(configuration.GetSection("Twilio"));

            return services;
        }
    }
}
