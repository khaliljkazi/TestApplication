using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Text;
using pdf = PdfSharp;
using pdfIO = PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Security;
using System.Net;
using System.Net.Http.Headers;

namespace TestApplication
{
    public static class PDFSecure
    {
        [FunctionName("PDFSecure")]
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            MemoryStream mem = new MemoryStream(File.ReadAllBytes(@"C:\Users\kkazi\Desktop\P000001186 - Copy.pdf"));
            //pdf.Pdf.PdfDocument document = pdf.Pdf.IO.PdfReader.Open(, pdfIO.PdfDocumentOpenMode.Modify);

            pdf.Pdf.PdfDocument document = pdf.Pdf.IO.PdfReader.Open(mem, pdfIO.PdfDocumentOpenMode.Modify);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1252 = Encoding.GetEncoding(1252);

            PdfSecuritySettings securitySettings = document.SecuritySettings;

            // Setting one of the passwords automatically sets the security level to 
            // PdfDocumentSecurityLevel.Encrypted128Bit.
            securitySettings.UserPassword = "user";
            securitySettings.OwnerPassword = "owner";

            // Don´t use 40 bit encryption unless needed for compatibility reasons
            //securitySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted40Bit;

            // Restrict some rights.
            securitySettings.PermitAccessibilityExtractContent = false;
            securitySettings.PermitAnnotations = false;
            securitySettings.PermitAssembleDocument = false;
            securitySettings.PermitExtractContent = false;
            securitySettings.PermitFormsFill = true;
            securitySettings.PermitFullQualityPrint = false;
            securitySettings.PermitModifyDocument = true;
            securitySettings.PermitPrint = false;
            MemoryStream ms = new MemoryStream();
            document.Save(ms);
            ms.Position = 0;
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(ms.ToArray());
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "PDFDocument.pdf"
            };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            return response;
        }
    }
}
