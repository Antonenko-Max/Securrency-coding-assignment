using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Securrency.TDS.Web.Services.HealthService
{
    static class ResponseWriteHelper
    {
        public static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, options))
                {
                    writer.WriteStartObject();
                    writer.WriteString("status", result.Status.ToString());
                    foreach (KeyValuePair<string, HealthReportEntry> entry in result.Entries)
                    {
                        writer.WriteString(entry.Key, entry.Value.Description);
                    }

                    writer.WriteEndObject();
                }

                stream.Position = 0;

                return stream.CopyToAsync(context.Response.Body);
            }
        }
    }
}
