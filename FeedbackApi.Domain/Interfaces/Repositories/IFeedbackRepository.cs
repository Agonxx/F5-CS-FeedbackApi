using FeedbackApi.Domain.DTOs;
using FeedbackApi.Domain.Entities;

namespace FeedbackApi.Domain.Interfaces.Repositories
{
    public interface IFeedbackRepository
    {
        Task<bool> ExistsByDoacao(int idDoacao);
        Task<bool> Create(Feedback feedback);
        Task<List<Feedback>> GetByDoador(int idDoador);
        Task<List<ResumoCampanhaAgregado>> GetResumoPorCampanha();
    }
}
