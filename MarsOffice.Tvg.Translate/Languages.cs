using System;
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
                var principal = MarsOfficePrincipal.Parse(req);
                var userId = principal.FindFirst("id").Value;

                return new OkObjectResult(new[] { 
                    "ro",
                    "en"
                });
            }
            catch (Exception e)
            {
                log.LogError(e, "Exception occured in function");
                return new BadRequestObjectResult(Errors.Extract(e));
            }
        }
    }
}