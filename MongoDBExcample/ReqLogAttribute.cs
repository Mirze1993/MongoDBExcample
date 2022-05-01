using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IO;

namespace MongoDBExcample
{
    public class ReqLogAttribute : ActionFilterAttribute
    {
        private readonly RecyclableMemoryStreamManager streamManager = new();
        private readonly Guid guid;


        public ReqLogAttribute()
        {
            guid = Guid.NewGuid();
        }

       

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.Request.EnableBuffering();
            context.HttpContext.Request.Body.Position = 0;
            using var reqStream = streamManager.GetStream();

            await context.HttpContext.Request.Body.CopyToAsync(reqStream);

            Console.WriteLine($"Http Request Information:{Environment.NewLine}" +
                           $"ID:{guid} " +
                           $"\n Schema:{context.HttpContext.Request.Scheme} " +
                           $"\n Host: {context.HttpContext.Request.Host} " +
                           $"\n Path: {context.HttpContext.Request.Path} " +
                           $"\n QueryString: {context.HttpContext.Request.QueryString} " +
                           $"\n Request Body: {await ReadStreamInChunks(reqStream)}");

            context.HttpContext.Request.Body.Position = 0;
            await base.OnActionExecutionAsync(context, next);
        }




        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var result = context.Result;
            string text = "";
            if (result is Microsoft.AspNetCore.Mvc.ObjectResult objectResult)
            {
                var status = objectResult.StatusCode;
                var stringResult = objectResult.Value ?? new();
                text = System.Text.Json.JsonSerializer.Serialize(stringResult);
            }
            Console.WriteLine($"\n Http Response Information:{Environment.NewLine}" +
                           $"ID:{guid} " +
                           $"\n Schema:{context.HttpContext.Request.Scheme} " +
                           $"\n Host: {context.HttpContext.Request.Host} " +
                           $"\n Path: {context.HttpContext.Request.Path} " +
                           $"\n QueryString: {context.HttpContext.Request.QueryString} " +
                           $"\n Response Body: {text}");
        }


        private static async Task<string> ReadStreamInChunks(Stream requestStream)
        {
            const int readChunkBufferLenght = 4096;
            requestStream.Seek(0, SeekOrigin.Begin);// requestStream.Position = 0;   

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(requestStream);

            var readChunk = new char[readChunkBufferLenght];

            int readChunkLenght;
            do
            {
                readChunkLenght = await reader.ReadBlockAsync(readChunk, 0, readChunkBufferLenght);
                textWriter.Write(readChunk, 0, readChunkLenght);

            } while (readChunkLenght > 0);
            return textWriter.ToString();
        }
    }
}
