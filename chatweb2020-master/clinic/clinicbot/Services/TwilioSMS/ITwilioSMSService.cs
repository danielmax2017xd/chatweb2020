using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace clinicbot.Services.TwilioSMS
{
    public interface ITwilioSMSService
    {
        Task<bool> SendMessage(string SMS, string number);
    }
}
