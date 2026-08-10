using DiyMusicCommunity.Application;
using DiyMusicCommunity.Infrastructure;
using DiyMusicCommunity.Api.Swagger;
using DiyMusicCommunity.Api.Converters;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // FormatJsonConverter must be registered first so it takes priority over the
        // global JsonStringEnumConverter for the Format enum specifically.
        options.JsonSerializerOptions.Converters.Add(new FormatJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddSwaggerDocumentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwaggerDocumentation();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

// Expose Program to the integration test project
public partial class Program { }
