using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Text;
using BackendSF.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace BackendSF
{
    internal sealed class BackendSF : StatelessService
    {
        public BackendSF(StatelessServiceContext context)
            : base(context) { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var builder = WebApplication.CreateBuilder();
                        builder.Services.AddScoped<SharedLinkAccessFilter>();

                        // Konfiguracija se ucitava iz appsettings.json
                        var configuration = builder.Configuration;

                        var jwtSecret = configuration["Jwt:Secret"]
                            ?? throw new InvalidOperationException("Jwt:Secret nije konfigurisan u appsettings.json");
                        var jwtIssuer = configuration["Jwt:Issuer"]
                            ?? throw new InvalidOperationException("Jwt:Issuer nije konfigurisan u appsettings.json");
                        var jwtAudience = configuration["Jwt:Audience"]
                            ?? throw new InvalidOperationException("Jwt:Audience nije konfigurisan u appsettings.json");

                        // JWT autentifikacija
                        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                                    IssuerSigningKey = new SymmetricSecurityKey(
                                        Encoding.UTF8.GetBytes(jwtSecret))
                                };
                            });

                        builder.Services.AddAuthorization();

                        // CORS
                        builder.Services.AddCors(options =>
                        {
                            options.AddPolicy("AllowAll", policy =>
                                policy.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
                        });

                        builder.Services.AddControllers();
                        builder.Services.AddEndpointsApiExplorer();

                        // Swagger sa podrskom za Bearer token
                        builder.Services.AddSwaggerGen(c =>
                        {
                            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                            {
                                Name = "Authorization",
                                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                                Scheme = "Bearer",
                                BearerFormat = "JWT",
                                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                                Description = "Upisi: Bearer {token}"
                            });
                            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                            {
                                {
                                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                                    {
                                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                                        {
                                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                            Id = "Bearer"
                                        }
                                    },
                                    new string[] {}
                                }
                            });
                        });

                        builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);

                        builder.WebHost
                            .UseKestrel()
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                            .UseUrls(url);

                        var app = builder.Build();

                        app.UseSwagger();
                        app.UseSwaggerUI(c =>
                        {
                            c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelPlanner API v1");
                            c.RoutePrefix = string.Empty;
                        });

                        app.MapGet("/", () => Results.Redirect("/swagger"));

                        app.UseCors("AllowAll");
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.MapControllers();

                        return app;
                    }))
            };
        }
    }
}