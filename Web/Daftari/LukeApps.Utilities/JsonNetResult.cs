using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace LukeApps.Utilities
{
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Error
            };
        }

        public JsonSerializerSettings Settings { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals
                (context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(this.ContentType) ?
                "application/json" : this.ContentType;

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;
            else
                response.AddHeader("Content-Encoding", "gzip");

            if (this.Data == null)
                return;

            using (var memStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(memStream, CompressionMode.Compress, true))
                {
                    using (var streamWriter = new StreamWriter(zipStream))
                    {
                        using (var jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            var jsonSerializer = JsonSerializer.Create(this.Settings);

                            jsonSerializer.Serialize(jsonWriter, this.Data);
                        }
                    }
                }

                response.BinaryWrite(memStream.ToArray());
            }
        }
    }
}