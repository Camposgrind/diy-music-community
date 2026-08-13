using System.Reflection;
using DiyMusicCommunity.Application.Bands.GetBands;
using DiyMusicCommunity.Application.Common;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DiyMusicCommunity.Api.Swagger;

/// <summary>
/// Centralises all Swashbuckle / OpenAPI configuration.
/// Called from <c>Program.cs</c> via the extension methods.
/// No business logic — purely API documentation wiring.
/// </summary>
public static class SwaggerConfiguration
{
    private const string DocumentName = "v1";

    // -----------------------------------------------------------------------
    // Registration
    // -----------------------------------------------------------------------

    /// <summary>
    /// Registers Swashbuckle services and configures the OpenAPI document.
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(DocumentName, new OpenApiInfo
            {
                Title = "DIY Music Community API",
                Version = "v1",
                Description = """
                    REST API for the DIY Music Community platform.
                    Allows visitors to discover and filter underground/DIY bands
                    (Punk, Crust, Grindcore, Powerviolence, D-Beat).
                    """,
                Contact = new OpenApiContact
                {
                    Name = "DIY Music Community"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT"
                }
            });

            // JWT Bearer security definition
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token. Example: eyJhbGci..."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Include XML comments produced by GenerateDocumentationFile=true
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);

            // Describe enums as strings so the UI shows "Active" instead of "0"
            options.UseInlineDefinitionsForEnums();

            // Use short class names only — e.g. "BandListItemDto", "PagedResultBandListItemDto"
            options.CustomSchemaIds(type =>
            {
                if (!type.IsGenericType)
                    return type.Name;

                var baseName = type.Name[..type.Name.IndexOf('`')];
                var args = string.Concat(type.GetGenericArguments().Select(t => t.Name));
                return $"{baseName}{args}";
            });

            // Group endpoints by controller name
            options.TagActionsBy(api => new[]
            {
                api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "Default"
            });

            options.DocInclusionPredicate((_, _) => true);

            // Schema filters: mark required properties
            options.SchemaFilter<RequiredNotNullableSchemaFilter>();
        });

        return services;
    }

    // -----------------------------------------------------------------------
    // Middleware
    // -----------------------------------------------------------------------

    /// <summary>
    /// Registers the Swagger UI middleware. Only enabled outside Production.
    /// </summary>
    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        if (!app.Environment.IsProduction())
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/{DocumentName}/swagger.json", "DIY Music Community API v1");
                options.RoutePrefix = "swagger";
                options.DocumentTitle = "DIY Music Community — API Docs";
                options.DefaultModelsExpandDepth(2);
                options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);
                options.DisplayRequestDuration();
                options.EnableFilter();
                options.EnableTryItOutByDefault();
            });
        }

        return app;
    }
}

/// <summary>
/// Marks all non-nullable value-type properties as required in the OpenAPI schema,
/// which improves generated TypeScript client typings.
/// </summary>
internal sealed class RequiredNotNullableSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties is null)
            return;

        foreach (var (key, value) in schema.Properties)
        {
            if (!value.Nullable && value.Default is null)
                schema.Required.Add(key);
        }
    }
}
