using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using GloryFlorence.API.Common;
using GloryFlorence.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GloryFlorence.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during request execution.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred.";
            IEnumerable<string>? errors = null;

            switch (exception)
            {
                case NotFoundException notFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundEx.Message;
                    break;
                case KeyNotFoundException keyNotFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    message = keyNotFoundEx.Message;
                    break;
                case UnauthorizedException unauthorizedEx:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = unauthorizedEx.Message;
                    break;
                case BadRequestException badRequestEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = badRequestEx.Message;
                    break;
                case ValidationException validationEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = "Validation failed.";
                    errors = validationEx.Errors.Select(e => e.ErrorMessage);
                    break;
                case InvalidOperationException invalidOpEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = invalidOpEx.Message;
                    break;
                case ArgumentException argEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = argEx.Message;
                    break;
                case Microsoft.EntityFrameworkCore.DbUpdateException dbUpdateEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = "Database operation failed: " + (dbUpdateEx.InnerException?.Message ?? dbUpdateEx.Message);
                    break;
                default:
                    if (_env.IsDevelopment())
                    {
                        message = exception.Message;
                        errors = new[] { exception.StackTrace ?? string.Empty };
                    }
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var apiResponse = ApiResponse<object>.FailureResponse(errors ?? new[] { message }, message);
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var result = JsonSerializer.Serialize(apiResponse, jsonOptions);

            return context.Response.WriteAsync(result);
        }
    }
}
