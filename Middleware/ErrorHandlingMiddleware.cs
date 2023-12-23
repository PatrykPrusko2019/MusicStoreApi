using MusicStoreApi.Exceptions;

namespace MusicStoreApi.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(DuplicateValueException duplicateValueException)
            {
                context.Response.StatusCode = 409;
                await context.Response.WriteAsync(duplicateValueException.Message);
            }
            catch(ForbidException forbidException)
            {
                context.Response.StatusCode = 403;
            }
            catch(BadRequestException badReguestException)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(badReguestException.Message);
            }
            catch(NotFoundException notFoundException)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(notFoundException.Message);
            }
            catch (Exception e)
            {

                logger.LogError(e, e.Message);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong");
            }
        }
    }
}
