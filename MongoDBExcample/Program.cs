using Microsoft.Extensions.Options;
using MongoDBExcample;

var builder = WebApplication.CreateBuilder(args);
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();


//builder.Services.AddHttpLogging(o =>
//{
//    o.ResponseHeaders.Add("UID");
//    o.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestBody
//        | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseBody
//    | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestQuery;
//});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.Configure<MongoDatabase>(builder.Configuration.GetSection("MongoDatabase"));
//builder.Services.Configure<MongoDatabase>(c=>builder.Configuration.GetSection("").Bind(c));
builder.Services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<IOptions<MongoDatabase>>().Value);


var app = builder.Build();
//app.UseHttpLogging();
//app.UseReqRespMapMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Use( async (context, next) => {
    context.Request.EnableBuffering();
    await next();
});

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();
