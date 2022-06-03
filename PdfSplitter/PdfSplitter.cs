using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Aspose.Pdf;
using System.Linq;
using System.Collections.Generic;

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

                var pdfDocument = new Document(req.Body);
                var pages = pdfDocument.Pages.ToArray();

                foreach (var page in pages)
                {
                    using var ms = new MemoryStream();
                    var doc = new Document();
                    doc.Pages.Add(page);
                    doc.Save(ms, SaveFormat.Pdf);
                    result.Add(ms.ToArray());
                }

                return new OkObjectResult(result.ToArray());
            }
            catch (Exception ex)
            {
                log.LogInformation($"Unable to split pdf file. Error - {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}

