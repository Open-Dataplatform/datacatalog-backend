using System;
using System.Net;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Exceptions;
using DataCatalog.Api.Services.Egress;
using DataCatalog.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataCatalog.Api.Extensions
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;
        private readonly ICorrelationIdResolver _correlationIdResolver;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger, ICorrelationIdResolver correlationIdResolver)
        {
            _logger = logger;
            _correlationIdResolver = correlationIdResolver;
        }

        public void OnException(ExceptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var exception = context.Exception;
            var message = exception.Message;
            var correlationId = _correlationIdResolver.GetCorrelationId();
            var knownError = true;
            var exceptionDto = new ExceptionDto
            {
                CorrelationId = correlationId,
                Message = message
            };
            var objectResult = new ObjectResult(exceptionDto);
            switch (exception)
            {
                case EgressConfigurationException:
                case NotFoundException:
                    objectResult.StatusCode = (int) HttpStatusCode.NotFound;
                    break;
                case ValidationException:
                case ValidationExceptionCollection:
                    objectResult.StatusCode = (int) HttpStatusCode.BadRequest;
                    break;
                case EgressAuthorizationException:
                    objectResult.StatusCode = (int) HttpStatusCode.Forbidden;
                    break;
                case DbUpdateException:
                {
                    if (exception.InnerException != null)
                    {
                        message = $"{message}\r\nInnerException:\r\n{exception.InnerException.Message}";
                    }

                    exceptionDto.Message = message;
                    objectResult.StatusCode = (int) HttpStatusCode.InternalServerError;
                    break;
                }
                case GenericEgressException:
                    exceptionDto.Message = $"Unknown error while trying to contact the Egress Api. Egress exception message: {message}";
                    objectResult.StatusCode = (int) HttpStatusCode.FailedDependency;
                    break;
                default:
                    knownError = false;
                    exceptionDto.Message = "Internal Server Error occurred";
                    objectResult.StatusCode = (int) HttpStatusCode.InternalServerError;
                    break;
            }

            if (knownError)
            {
                _logger.LogInformation("Known error resulted in: {ExceptionMessage}", message);
            }
            else
            {
                _logger.LogError(exception, "Unknown error resulted in: {ExceptionMessage}", message);
            }

            context.Result = objectResult;
            context.ExceptionHandled = true;
        }
    }
}
