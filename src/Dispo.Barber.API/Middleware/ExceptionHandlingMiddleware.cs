using Dispo.Barber.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Dispo.Barber.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex) // Business Errors (400)
            {
                _logger.LogWarning(ex, "Erro de negócio: {Message}", ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (NotFoundException ex) // NotFound Errors (400)
            {
                _logger.LogWarning(ex, "Não encontrado: {Message}", ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (AlreadyExistsException ex) // AlreadyExists Errors (400)
            {
                _logger.LogWarning(ex, "Já existe : {Message}", ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (UnauthorizedAccessException ex) // Auth Errors (401)
            {
                _logger.LogWarning(ex, "Acesso não autorizado.");
                await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, "Acesso não autorizado.");
            }
            catch (Exception ex) // Unexpected Errors (500)
            {
                _logger.LogError(ex, "Erro interno no servidor.");
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Erro interno no servidor.");
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = context.Response.StatusCode,
                message
            }));
        }
    }
}
