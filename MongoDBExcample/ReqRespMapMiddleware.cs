using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IO;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBExcample
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ReqRespMapMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ReqRespMapMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager streamManager;
        public ReqRespMapMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory?.CreateLogger<ReqRespMapMiddleware>() ??
       throw new ArgumentNullException(nameof(loggerFactory));
            streamManager= new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext httpContext)
        {

            
            //await GetRequestBody(httpContext);
            //await LogResponse(httpContext);


            //httpContext.Request.EnableBuffering();
            await _next(httpContext);

            var endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<TelemetryAttribute>();

            if (attribute != null)
            {
                var ev = attribute.Event;
                await GetRequestBody(httpContext);

                //var body=await GetRequestBody(httpContext);
                //Console.WriteLine(body);
            }
        }


        private  async Task GetRequestBody(HttpContext context)
        {
            context.Request.EnableBuffering();

            using var reqStream= streamManager.GetStream();

            await context.Request.Body.CopyToAsync(reqStream);

            Console.WriteLine($"Http Request Information:{Environment.NewLine}" +
                           $"\nSchema:{context.Request.Scheme} " +
                           $"\nHost: {context.Request.Host} " +
                           $"\nPath: {context.Request.Path} " +
                           $"\nQueryString: {context.Request.QueryString} " +
                           $"\nRequest Body: {await ReadStreamInChunks(reqStream)}");

            context.Request.Body.Position = 0;
        }

        private static async Task<string> ReadStreamInChunks(Stream requestStream)
        {
            const int readChunkBufferLenght = 4096;
            requestStream.Seek(0, SeekOrigin.Begin);// requestStream.Position = 0;   

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(requestStream);

            var readChunk=new char[readChunkBufferLenght];

            int readChunkLenght;
            do
            {
                readChunkLenght= await reader.ReadBlockAsync(readChunk, 0, readChunkBufferLenght); 
                textWriter.Write(readChunk,0, readChunkLenght);

            } while (readChunkLenght>0);
            return textWriter.ToString();   

        }


        private async Task LogResponse(HttpContext context)
        {
            var orginalBodySream = context.Response.Body;

            using var respBody = streamManager.GetStream();
            context.Response.Body = respBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var text=await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);    

            Console.WriteLine($"\n Http Response Information:{Environment.NewLine}" +
                           $"\n Schema:{context.Request.Scheme} " +
                           $"\n Host: {context.Request.Host} " +
                           $"\n Path: {context.Request.Path} " +
                           $"\n QueryString: {context.Request.QueryString} " +
                           $"\n Response Body: {text}");

            await respBody.CopyToAsync(orginalBodySream);
        }






        //private static async Task<string> GetRequestBody(HttpContext context)
        //{
            
        //    var req = context.Request;
        //    req.Body.Position = 0;
        //    using StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        //    return await reader.ReadToEndAsync();

        //}
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ReqRespMapMiddlewareExtensions
    {
        public static IApplicationBuilder UseReqRespMapMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ReqRespMapMiddleware>();
        }
    }

    

   
}
