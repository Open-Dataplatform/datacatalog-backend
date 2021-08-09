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
        /// <summary>
        /// Class used to customize how type names are generated by NSwag
        /// </summary>
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
            app.UseOpenApi(config => config.DocumentName = "v1.0");
            app.UseSwaggerUi3(config => config.DocExpansion = "list");
        }
    }
}