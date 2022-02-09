using System;
using System.Linq;
using System.Threading.Tasks;
using MarsOffice.Microfunction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MarsOffice.Tvg.Translate
{
    public class Languages
    {

        public Languages()
        {
        }

        [FunctionName("GetAllLanguages")]
        public async Task<IActionResult> GetAllLanguages(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/translate/getAllLanguages")] HttpRequest req,
            ILogger log
            )
        {
            try
            {
                await Task.CompletedTask;
                return new OkObjectResult(new[] { 
                    "ro",
                    "en",
                    "af",
                    "sq",
                    "bg",
                    "ca",
                    "zh-Hans",
                    "hr",
                    "cs",
                    "da",
                    "nl",
                    "et",
                    "fi",
                    "de",
                    "el",
                    "he",
                    "hi",
                    "hu",
                    "is",
                    "id",
                    "ga",
                    "it",
                    "ja",
                    "ko",
                    "lv",
                    "lt",
                    "mk",
                    "nb",
                    "pt",
                    "pt-pt",
                    "pl",
                    "ru",
                    "sk",
                    "sl",
                    "es",
                    "sv",
                    "th",
                    "uk",
                    "tr",
                    "vi",
                    "cy",
                    "ur"
                }.Distinct().OrderByDescending(x => x).ToList());
            }
            catch (Exception e)
            {
                log.LogError(e, "Exception occured in function");
                return new BadRequestObjectResult(Errors.Extract(e));
            }
        }
    }
}