using Transportation.Interfaces.IApplicationServices;
using Transportation.Interfaces.IHelpersServices;
using Transportation.Interfaces.IIdentityServices;
using Transportation.Services.ApplicationServices;
using Transportation.Services.HelperServices;
using Transportation.Services.IdentityServices;

namespace Transportation.API.Helpers.DI
{
    public static class ServicesDi
    {
        public static IServiceCollection AddServicesDi(this IServiceCollection services)
        {
            services.AddScoped<IBusServices, BusServices>();
            services.AddScoped<IJourneysHistoryServices, JourneysHistoryServices>();
            services.AddScoped<ISeatServices, SeatServices>();
            services.AddScoped<ITicketServices, TicketServices>();
            services.AddScoped<IUpcomingJourneysServices, UpcomingJourneysServices>();

            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IManagerServices, ManagerServices>();
            services.AddScoped<IAdminServices, AdminServices>();
            services.AddScoped<IMailServices, MailServices>();
            services.AddScoped<ISmsSevices, SmsServices>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
