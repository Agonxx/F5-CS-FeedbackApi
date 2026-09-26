using FeedbackApi.Domain.DTOs;

namespace FeedbackApi.Domain.Interfaces.Services
{
    public interface IFeedbackService
    {
        Task<FeedbackResponse> EnviarAsync(FeedbackRequest request);
        Task<List<FeedbackResponse>> GetMeusAsync();
        Task<List<ResumoCampanhaResponse>> GetResumoAsync();
    }
}
