using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace PdfSplitter
{
    public static class PdfSplitter
    {
        [FunctionName("PdfSplitter")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var result = new List<byte[]>();

                var inputDocument = PdfReader.Open(req.Body, PdfDocumentOpenMode.Import);

                for (var idx = 0; idx < inputDocument.PageCount; idx++)
                {
                    using (var ms = new MemoryStream())
                    {
                        var outputDocument = new PdfDocument();
                        outputDocument.Version = inputDocument.Version;

                        // Add the page and save it.
                        outputDocument.AddPage(inputDocument.Pages[idx]);
                        outputDocument.Save(ms);
                        result.Add(ms.ToArray());
                    }
                }

                return new OkObjectResult(result.ToArray());
            }
            catch (Exception ex)
            {
                log.LogInformation($"Unable to split pdf file. Error - {ex.Message}");
                return new BadRequestObjectResult($"Message - {ex.Message}. StackTrace - {ex.StackTrace}");
            }
        }
    }
}

