﻿using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using NJsonSchema;

namespace DataCatalog.Api.Extensions
{
    public static class SwaggerExtension
    {
        internal class CustomTypeNameGenerator : DefaultTypeNameGenerator, ITypeNameGenerator
        {
            public override string Generate(JsonSchema schema, string typeNameHint, IEnumerable<string> reservedTypeNames)
            {
                var typeName = base.Generate(schema, typeNameHint, reservedTypeNames);

                // Remove trailing 'Response' from type names
                if (typeName.EndsWith("Response"))
                    typeName = typeName.Replace("Response", "");

                return typeName;
            }
        }

        /// <summary>
        /// Initial swagger setup
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwagger(this IServiceCollection services)
        {
            // services.AddSwaggerGen(options =>
            // {
            //     options.SwaggerDoc("v1.0", new OpenApiInfo
            //     {
            //         Title = "DataCatalog.Api endpoint",
            //         Version = "v1.0",
            //         Contact = new OpenApiContact()
            //         {
            //             Name = "Dataplatform team",
            //             Email = "dataplatform@energinet.dk"
            //         }
            //     });
            //     options.DocInclusionPredicate((docName, apiDesc) =>
            //     {
            //         var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersionModel();
            //         if (actionApiVersionModel == null)
            //         {
            //             return true;
            //         }
            //         if (actionApiVersionModel.DeclaredApiVersions.Any())
            //         {
            //             return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
            //         }
            //         return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
            //     });
            //     options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            //     //options.OperationFilter<AddRequiredHeaderParameter>();

            //     // Configure Swagger to use the xml documentation file
            //     var xmlFile = Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml");
            //     options.IncludeXmlComments(xmlFile);
            // });

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1.0";
                    document.Info.Title = "DataCatalog.Api endpoint";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Dataplatform team",
                        Email = "dataplatform@energinet.dk",
                    };
                };
                config.TypeNameGenerator = new CustomTypeNameGenerator();
            });
        }

        /// <summary>
        /// Swagger endpoint setup
        /// </summary>
        /// <param name="app"></param>
        public static void UseCustomSwagger(this IApplicationBuilder app)
        {
            // app.UseSwagger();
            // app.UseSwaggerUI(c =>
            // {
            //     c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "API v1.0");
            // });

            app.UseOpenApi(config => config.DocumentName = "v1.0");
            app.UseSwaggerUi3(config => config.DocExpansion = "list");
        }
        
        /// <summary>
        /// Header declaration on swagger documentation.
        /// </summary>
        // public class AddRequiredHeaderParameter : IOperationFilter
        // {
        //     public void Apply(OpenApiOperation operation, OperationFilterContext context)
        //     {
        //         if (operation.Parameters == null)
        //             operation.Parameters = new List<OpenApiParameter>();

        //         operation.Parameters.Add(new OpenApiParameter()
        //         {
        //             Name = "X-Version",
        //             Description = "Request api version.",
        //             Required = true
        //         });
        //     }
        // }
    }
}