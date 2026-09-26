using FeedbackApi.Domain;
using FeedbackApi.Domain.DTOs;
using FeedbackApi.Domain.Extensions;

namespace FeedbackApi.Api.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, InfoToken _infoToken)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                // O token é repassado à CampanhasApi para conferir de quem é a doação
                _infoToken.Token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
                _infoToken.Id = int.Parse(context.User.FindFirst(nameof(InfoToken.Id)).Value);
                _infoToken.Nome = context.User.FindFirst(nameof(InfoToken.Nome)).Value;
                _infoToken.Email = context.User.FindFirst(nameof(InfoToken.Email)).Value;
                _infoToken.Role = EnumExtensions.ToEnum<ERole>(context.User.FindFirst(nameof(InfoToken.Role)).Value);
                _infoToken.CadastradoEm = DateTime.Parse(context.User.FindFirst(nameof(InfoToken.CadastradoEm)).Value);
            }

            await _next(context);
        }
    }
}
