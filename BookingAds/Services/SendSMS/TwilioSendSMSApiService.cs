using System;
using BookingAds.Services.Abstractions;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace BookingAds.Services
{
    public class TwilioSendSMSApiService : ISendSMSService
    {
        private const string FromPhone = "+17726751887";
        private const string AccountSid = "AC773108fea10318cd0f304ea1eac1fb27";
        private const string AuthToken = "224d4118492ca528dfe4cf8d1bc795cd";

        public string SendSMS(string phone, string msg)
        {
            try
            {
                // init twilio client
                TwilioClient.Init(AccountSid, AuthToken);

                // set message options
                var fromPhone = new PhoneNumber(FromPhone);
                var toPhone = new PhoneNumber(phone);
                var messageOptions = new CreateMessageOptions(toPhone)
                {
                    From = fromPhone,
                    Body = msg,
                };

                // create message
                var message = MessageResource.Create(messageOptions);

                // test
                Console.WriteLine($"From phone {fromPhone} send sms to phone {toPhone} with content: {message.Body}");

                return message.Body;
            }
            catch
            {
                return null;
            }
        }
    }
}