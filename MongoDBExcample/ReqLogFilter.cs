using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IO;

namespace MongoDBExcample
{
    public class ReqLogFilter : Attribute, IAsyncActionFilter
    {
        private readonly RecyclableMemoryStreamManager streamManager=new();

     


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

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var result = await next();



            result.HttpContext.Request.EnableBuffering();
            result.HttpContext.Request.Body.Position = 0;
            using var reqStream = streamManager.GetStream();

            await result.HttpContext.Request.Body.CopyToAsync(reqStream);

            Console.WriteLine($"Http Request Information:{Environment.NewLine}" +
                           $"\nSchema:{result.HttpContext.Request.Scheme} " +
                           $"\nHost: {result.HttpContext.Request.Host} " +
                           $"\nPath: {result.HttpContext.Request.Path} " +
                           $"\nQueryString: {result.HttpContext.Request.QueryString} " +
                           $"\nRequest Body: {await ReadStreamInChunks(reqStream)}");

            result.HttpContext.Request.Body.Position = 0;

        }
    }
}
