using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Attendances.Shared.Commons.Configurations;

public record SwaggerSettings(string SchemeName);

public static class SwaggerConfiguration
{
    private static readonly string BearerHeaderName = "X-Authorization";
    internal static Task<IServiceCollection> AddAppSwagger(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var swaggerSettings = configuration.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();
        
        services.AddOptions<SwaggerGenOptions>();
        services.AddSwaggerGen(options =>
        {
            options.SupportNonNullableReferenceTypes();
            options.UseInlineDefinitionsForEnums();
            options.DescribeAllParametersInCamelCase();
            options.CustomSchemaIds(x => x.FullName);
            
            var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml");
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Insert JWT with Bearer",
                Name = BearerHeaderName,
                BearerFormat = "JWT",
                Type = SecuritySchemeType.ApiKey,
                Scheme = swaggerSettings?.SchemeName ?? "Bearer"
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
                    new List<string>()
                }
            });

            options.EnableAnnotations();
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = $"{Assembly.GetEntryAssembly()?.GetName().Name} API",
            });
            options.UseOneOfForPolymorphism();
            options.EnableAnnotations(true, true);

            options.UseAllOfForInheritance();
            options.UseOneOfForPolymorphism();

            options.ExampleFilters();
        });
        services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
        services.AddSwaggerGenNewtonsoftSupport();
        return Task.FromResult(services);
    }
    internal static WebApplication UseAppSwagger(this WebApplication application)
    {
        application.UseSwagger();
        application.UseSwaggerUI(
            options =>
            {
                options.DocExpansion(DocExpansion.List);
                options.DefaultModelsExpandDepth(-1);
            }
        );
        return application;
    }
}
