using Microsoft.Extensions.Options;
using Transportation.Core.Helpers.Classes;
using Transportation.Interfaces.IHelpersServices;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Transportation.Services.HelperServices
{
    public class SmsServices(IOptions<TwilioConfiguration> options) : ISmsSevices
    {
        private readonly TwilioConfiguration _options = options.Value;

        public string GenerateCode()
        {
            var code = "";
            for (var i = 0; i < 5; i++)
                code += new Random().Next(9);
            return code;
        }

        public MessageResource Send(string message, string phoneNumber)
        {
            TwilioClient.Init(_options.AccountSID, _options.AuthToken);

            var response = MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber(_options.PhoneNumber),
                to: phoneNumber);

            return response;
        }


    }
}
