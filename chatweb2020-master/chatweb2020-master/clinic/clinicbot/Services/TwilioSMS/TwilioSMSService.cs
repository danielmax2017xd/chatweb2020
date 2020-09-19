using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace clinicbot.Services.TwilioSMS
{
    public class TwilioSMSService: ITwilioSMSService
    {
        public async Task<bool> SendMessage(string SMS, string number)
        {
            try
            {
                string accountSid = "ACc234564956681bf682dcc4f3437b4a7e";
                string authToken = "1b188f4909d19efa86f1c4eab52075d2";
                
                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    body: SMS,
                    from: new Twilio.Types.PhoneNumber("+12184232151"),
                    to: new Twilio.Types.PhoneNumber($"+51{number}")
                );

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
