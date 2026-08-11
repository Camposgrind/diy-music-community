using DiyMusicCommunity.Application;
using DiyMusicCommunity.Infrastructure;
using DiyMusicCommunity.Api.Swagger;
using DiyMusicCommunity.Api.Converters;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new FormatJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerDocumentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwaggerDocumentation();
app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

// Expose Program to the integration test project
public partial class Program { }
