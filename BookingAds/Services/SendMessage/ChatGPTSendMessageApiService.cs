using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using BookingAds.Common.Models.Message;
using BookingAds.Services.Abstractions;
using Newtonsoft.Json;
using RestSharp;

namespace BookingAds.Services
{
    public class ChatGPTSendMessageApiService : ISendMessageService
    {
        private const string ApiKey = "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";
        private const string AuthorizationKey = "Authorization";
        private const string AuthorizationType = "Bearer";
        private readonly string AuthorizationValue = $"{AuthorizationType} {ApiKey}";
        private const string ContentTypeKey = "Content-Type";
        private const string ContentTypeValue = "application/json";
        private const string CreateChatCompletionURL = "https://api.openai.com/v1/chat/completions";
        private const string DefaultModel = "gpt-3.5-turbo";
        private const string DefaultRoleQuestion = "user";

        public string SendMessage(string msg)
        {
            using (var httpClient = new RestClient(CreateChatCompletionURL))
            {
                var request = new RestRequest(string.Empty, Method.Post);

                request.AddHeader(AuthorizationKey, AuthorizationValue);
                request.AddHeader(ContentTypeKey, ContentTypeValue);

                var messageRequest = new MessageRequest()
                {
                    Role = DefaultRoleQuestion,
                    Content = msg,
                };

                var requestBody = new ViewChatGPTRequest
                {
                    Model = DefaultModel,
                    Messages = new List<MessageRequest>()
                    {
                        messageRequest,
                    },
                };

                var requestJsonBody = JsonConvert.SerializeObject(requestBody).ToLower().Trim();

                request.AddJsonBody(requestJsonBody);

                var response = httpClient.Execute(request, Method.Post);

                var jsonResponse = JsonConvert.DeserializeObject<ViewChatGPTResponse>(response.Content ?? string.Empty);

                var answer = jsonResponse?.Choices[0]?.Message.Content?.ToString()?.Trim() ?? string.Empty;

                return answer;
            }
        }
    }
}