using Microsoft.Extensions.Options;
using MongoDBExcample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.Configure<MongoDatabase>(builder.Configuration.GetSection("MongoDatabase"));
//builder.Services.Configure<MongoDatabase>(c=>builder.Configuration.GetSection("").Bind(c));
builder.Services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<IOptions<MongoDatabase>>().Value);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();
