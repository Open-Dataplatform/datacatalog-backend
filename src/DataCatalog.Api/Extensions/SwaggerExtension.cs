using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace DataCatalog.Api.Extensions
{
    public static class SwaggerExtension
    {
        /// <summary>
        /// Initial swagger setup
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new OpenApiInfo
                {
                    Title = "DataCatalog.Api endpoint",
                    Version = "v1.0",
                    Contact = new OpenApiContact()
                    {
                        Name = "Dataplatform team",
                        Email = "dataplatform@energinet.dk"
                    }
                });
                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersionModel();
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                //options.OperationFilter<AddRequiredHeaderParameter>();

                // Configure Swagger to use the xml documentation file
                var xmlFile = Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml");
                options.IncludeXmlComments(xmlFile);
            });
        }

        /// <summary>
        /// Swagger endpoint setup
        /// </summary>
        /// <param name="app"></param>
        public static void UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "API v1.0");
            });
        }
        
        /// <summary>
        /// Header declaration on swagger documentation.
        /// </summary>
        public class AddRequiredHeaderParameter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "X-Version",
                    Description = "Request api version.",
                    Required = true
                });
            }
        }
    }
}