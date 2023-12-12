using System.Diagnostics;

namespace MusicStoreApi.Middleware
{
    public class RequestTimeMiddleware : IMiddleware
    {
        private readonly ILogger<RequestTimeMiddleware> logger;
        private Stopwatch stopwatch;

        public RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger)
        {
            this.logger = logger;
            this.stopwatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            stopwatch.Start();
            await next.Invoke(context);
            stopwatch.Stop();

            if (stopwatch.Elapsed.Seconds > 4)
            {
                var message = $"Request [{context.Request.Method}] at {context.Request.Path} took {stopwatch.Elapsed.Seconds} seconds";
                logger.LogInformation(message);
            }
        }
    }
}
