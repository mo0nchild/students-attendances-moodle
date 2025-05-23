
using Attendances.Shared.Commons;
using Attendances.Shared.Security;
using Attendances.System.WebApi.Configurations;
using Newtonsoft.Json.Converters;

namespace Attendances.System.WebApi;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers().AddNewtonsoftJson(opts =>
        {
            opts.SerializerSettings.Converters.Add(new StringEnumConverter());
        });
        builder.Services.AddHttpClient();
        builder.Services.AddHealthChecks();
        builder.Services.AddEndpointsApiExplorer();

        await builder.Services.AddSecurityServices(builder.Configuration);
        await builder.Services.AddSecretService(builder.Configuration);
        await builder.Services.AddApiServices(builder.Configuration);
        await builder.Services.AddCoreConfiguration(builder.Configuration);

        var application = builder.Build();
        
        if (application.Environment.IsDevelopment())
        {
            application.UseSwagger();
            application.UseSwaggerUI();
        }
        application.UseHttpsRedirection();
        application.UseCoreConfiguration();
        application.UseSecurity();
        
        application.UseHealthChecks("/health");
        application.MapControllers();

        await application.RunAsync();
    }
}