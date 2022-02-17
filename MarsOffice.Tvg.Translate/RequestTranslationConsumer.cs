using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MarsOffice.Tvg.Translate.Abstractions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;

namespace MarsOffice.Tvg.Translate
{
    class AzureAiTranslateRequest
    {
        public string Text { get; set; }
    }

    class AzureAiTranslationResponse
    {
        public string Text { get; set; }
        public string To { get; set; }
    }

    class AzureAiTranslateResponse
    {
        public IEnumerable<AzureAiTranslationResponse> Translations { get; set; }
    }

    public class RequestTranslationConsumer
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public RequestTranslationConsumer(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient();
            _config = config;
        }

        [FunctionName("RequestTranslationConsumer")]
        public async Task Run(
            [QueueTrigger("request-translation", Connection = "localsaconnectionstring")] RequestTranslation request,
            [Queue("translation-response", Connection = "localsaconnectionstring")] IAsyncCollector<TranslationResponse> translationResponseQueue,
            ILogger log)
        {
            try
            {
                var response = await Policy
                        .Handle<Exception>()
                        .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                        .ExecuteAsync(async () =>
                        {
                            var jsonPayload = JsonConvert.SerializeObject(request.Sentences.Select(s => new AzureAiTranslateRequest
                            {
                                Text = s
                            }).ToList(), new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            });
                            var stringContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_config["aiendpoint"]}translate?api-version=3.0&from={request.FromLangCode}&to={request.ToLangCode}");
                            httpRequest.Content = stringContent;
                            httpRequest.Headers.Add("Ocp-Apim-Subscription-Key", _config["aikey"]);
                            httpRequest.Headers.Add("Ocp-Apim-Subscription-Region", _config["location"].Replace(" ", "").ToLower());

                            var httpResponse = await _httpClient.SendAsync(httpRequest);
                            httpResponse.EnsureSuccessStatusCode();
                            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                            var deserialized = JsonConvert.DeserializeObject<IEnumerable<AzureAiTranslateResponse>>(jsonResponse, new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            });
                            return deserialized.Select(x => x.Translations.First().Text).ToList();
                        });


                await translationResponseQueue.AddAsync(new TranslationResponse
                {
                    Success = true,
                    VideoId = request.VideoId,
                    JobId = request.JobId,
                    UserId = request.UserId,
                    UserEmail = request.UserEmail,
                    FromLangCode = request.FromLangCode,
                    ToLangCode = request.ToLangCode,
                    TranslatedSentences = response
                });
                await translationResponseQueue.FlushAsync();
            }
            catch (Exception e)
            {
                log.LogError(e, "Function threw an exception");
                await translationResponseQueue.AddAsync(new TranslationResponse
                {
                    Error = e.Message,
                    Success = false,
                    VideoId = request.VideoId,
                    JobId = request.JobId,
                    UserId = request.UserId,
                    UserEmail = request.UserEmail,
                    FromLangCode = request.FromLangCode,
                    ToLangCode = request.ToLangCode
                });
                await translationResponseQueue.FlushAsync();
                throw;
            }
        }
    }
}
