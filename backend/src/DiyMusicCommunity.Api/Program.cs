using DiyMusicCommunity.Application;
using DiyMusicCommunity.Infrastructure;
using DiyMusicCommunity.Api.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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
