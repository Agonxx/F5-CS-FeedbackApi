using FeedbackApi.Domain.Constants;
using FeedbackApi.Domain.DTOs;
using FeedbackApi.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FeedbackApi.Api.Controllers
{
    public class FeedbackController : BaseController
    {
        private readonly IFeedbackService _service;

        public FeedbackController(IHttpContextAccessor httpContextAccessor,
                                    IFeedbackService service,
                                    InfoToken infoToken) : base(httpContextAccessor, infoToken)
        {
            _service = service;
        }

        [HttpPost(FeedbackRoutes.Enviar)]
        [RoleAuthorize(Roles.DoadorAccess)]
        public async Task<IActionResult> Enviar([FromBody] FeedbackRequest request)
        {
            var feedback = await _service.EnviarAsync(request);
            return Ok(feedback);
        }

        [HttpGet(FeedbackRoutes.Meus)]
        [RoleAuthorize(Roles.DoadorAccess)]
        public async Task<IActionResult> Meus()
        {
            var feedbacks = await _service.GetMeusAsync();
            return Ok(feedbacks);
        }

        [HttpGet(FeedbackRoutes.Resumo)]
        [RoleAuthorize(Roles.GestorAccess)]
        public async Task<IActionResult> Resumo()
        {
            var resumo = await _service.GetResumoAsync();
            return Ok(resumo);
        }
    }
}
