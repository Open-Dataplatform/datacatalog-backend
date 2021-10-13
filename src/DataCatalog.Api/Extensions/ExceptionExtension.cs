using DataCatalog.Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DataCatalog.Api.Extensions
{
    public class ExceptionExtension
    {
        readonly RequestDelegate _next;
        readonly ILogger<ExceptionExtension> _logger;

        public ExceptionExtension(RequestDelegate next, ILogger<ExceptionExtension> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var message = ex.Message;
            _logger.LogError(ex, message);

            switch (ex)
            {
                case NotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case ValidationException:
                case ValidationExceptionCollection:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case DbUpdateException:
                {
                    if (ex.InnerException != null)
                        message = $"{message}\r\nInnerException:\r\n{ex.InnerException.Message}";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
                }
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            await context.Response.WriteAsync(message);
        }
    }
}
