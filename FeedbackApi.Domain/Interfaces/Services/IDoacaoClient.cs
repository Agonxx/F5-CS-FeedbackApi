using FeedbackApi.Domain.DTOs;

namespace FeedbackApi.Domain.Interfaces.Services
{
    public interface IDoacaoClient
    {
        /// <summary>Doações do doador logado, consultadas na CampanhasApi com o JWT da requisição.</summary>
        Task<List<DoacaoInfo>> GetMinhasDoacoes();
    }
}
