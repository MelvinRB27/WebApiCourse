namespace WebApiCourse.Middleware
{

    public static class LoggerResponseMiddlewareExtension
    {
        public static IApplicationBuilder useLoggerResponse(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggerResponseMiddleware>();
        }
    }

    public class LoggerResponseMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LoggerResponseMiddleware> logger;

        public LoggerResponseMiddleware(RequestDelegate next, ILogger<LoggerResponseMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var bodyOriginalResponse = context.Response.Body;
                context.Response.Body = ms;

                await next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(bodyOriginalResponse);
                context.Response.Body = bodyOriginalResponse;

                logger.LogInformation(response);
            }
        }
    }
}
