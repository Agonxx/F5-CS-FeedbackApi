namespace FeedbackApi.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado capturado pelo ExceptionMiddleware.");

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                object resposta;

                if (_environment.IsDevelopment())
                {
                    resposta = new
                    {
                        message = ex.Message,
                        stackTrace = ex.StackTrace
                    };
                }
                else
                {
                    resposta = new
                    {
                        message = ex.Message
                    };
                }

                await context.Response.WriteAsJsonAsync(resposta);
            }
        }
    }
}
