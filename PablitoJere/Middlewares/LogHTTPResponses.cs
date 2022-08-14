namespace PablitoJere.Middlewares
{
    public static class LoguearRespuestaHTTPMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogHTTPResponses(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogHTTPResponses>();
        }
    }

    public class LogHTTPResponses
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LogHTTPResponses> logger;

        public LogHTTPResponses(RequestDelegate next,
            ILogger<LogHTTPResponses> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        // Invoke o InvokeAsync
        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRespuesta = context.Response.Body;
                context.Response.Body = ms;

                await next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                context.Response.Body = cuerpoOriginalRespuesta;

                logger.LogInformation(response);
            }
        }
    }
}
