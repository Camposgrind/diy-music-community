using System.Text;
using Azure.Identity;
using DiyMusicCommunity.Application;
using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Infrastructure;
using DiyMusicCommunity.Infrastructure.Auth;
using DiyMusicCommunity.Infrastructure.Persistence;
using DiyMusicCommunity.Infrastructure.Persistence.Seed;
using DiyMusicCommunity.Api.Services;
using DiyMusicCommunity.Api.Swagger;
using DiyMusicCommunity.Api.Converters;
using DiyMusicCommunity.Api.Telemetry;
using DiyMusicCommunity.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = builder.Configuration["AzureKeyVaultEndpoint"];
if (Uri.TryCreate(keyVaultEndpoint, UriKind.Absolute, out var keyVaultUri))
{
    builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
}

var applicationInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrWhiteSpace(applicationInsightsConnectionString) &&
    !string.Equals(applicationInsightsConnectionString, "SET_VIA_KEYVAULT_OR_USER_SECRETS", StringComparison.Ordinal))
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = applicationInsightsConnectionString;
    });
    builder.Services.AddSingleton<IApplicationTelemetry, ApplicationInsightsTelemetry>();
}
else
{
    builder.Services.AddSingleton<IApplicationTelemetry, NullApplicationTelemetry>();
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new FormatJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var maxImageSizeMb = builder.Configuration.GetValue<int>("FileUpload:MaxImageSizeMb");
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = (long)maxImageSizeMb * 1024 * 1024;
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

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer is not configured.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT Audience is not configured.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorizationBuilder();

var app = builder.Build();

// Apply all pending EF Core migrations automatically on startup.
// This makes every environment (dev, staging, prod) self-migrate on deploy.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsRelational())
    {
        await db.Database.MigrateAsync();
    }
}

// Seed roles first — roles must exist before assigning them to users
await RoleSeeder.SeedRolesAsync(app.Services);

// Promote the configured admin email to Admin role (set Seed:AdminEmail via user-secrets)
await AdminSeeder.SeedAdminAsync(app.Services);

app.UseSwaggerDocumentation();
app.UseMiddleware<ExceptionLoggingMiddleware>();
app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Expose Program to the integration test project
public partial class Program { }


