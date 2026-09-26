using FeedbackApi.Domain;
using FeedbackApi.Domain.DTOs;
using FeedbackApi.Domain.Entities;
using FeedbackApi.Domain.Interfaces.Repositories;
using FeedbackApi.Domain.Interfaces.Services;

namespace FeedbackApi.Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _repo;
        private readonly IDoacaoClient _doacaoClient;
        private readonly InfoToken _infoToken;

        public FeedbackService(IFeedbackRepository repo, IDoacaoClient doacaoClient, InfoToken infoToken)
        {
            _repo = repo;
            _doacaoClient = doacaoClient;
            _infoToken = infoToken;
        }

        public async Task<FeedbackResponse> EnviarAsync(FeedbackRequest request)
        {
            if (request.Rapidez < 1 || request.Rapidez > 5)
                throw new Exception("A nota de rapidez deve ser de 1 a 5");

            if (request.Dificuldade < 1 || request.Dificuldade > 5)
                throw new Exception("A nota de dificuldade deve ser de 1 a 5");

            if (!Enum.IsDefined(request.PretendeVoltar))
                throw new Exception("Informe se pretende voltar a doar: Sim, Talvez ou Nao");

            // A doação mora na CampanhasApi: só o dono dela (pelo JWT) consegue achá-la aqui
            var doacoes = await _doacaoClient.GetMinhasDoacoes();
            var doacao = doacoes.FirstOrDefault(d => d.Id == request.IdDoacao);

            if (doacao is null)
                throw new Exception("Doação não encontrada para o doador logado");

            if (await _repo.ExistsByDoacao(doacao.Id))
                throw new Exception("Já existe um feedback para esta doação");

            var feedback = new Feedback
            {
                IdDoacao = doacao.Id,
                IdCampanha = doacao.IdCampanha,
                IdDoador = _infoToken.Id,
                Rapidez = request.Rapidez,
                Dificuldade = request.Dificuldade,
                PretendeVoltar = request.PretendeVoltar,
                Comentario = request.Comentario
            };

            await _repo.Create(feedback);

            return ToResponse(feedback);
        }

        public async Task<List<FeedbackResponse>> GetMeusAsync()
        {
            var feedbacks = await _repo.GetByDoador(_infoToken.Id);
            return feedbacks.Select(ToResponse).ToList();
        }

        public async Task<List<ResumoCampanhaResponse>> GetResumoAsync()
        {
            var agregados = await _repo.GetResumoPorCampanha();

            return agregados
                .OrderBy(a => a.IdCampanha)
                .Select(a => new ResumoCampanhaResponse
                {
                    IdCampanha = a.IdCampanha,
                    TotalFeedbacks = a.TotalFeedbacks,
                    MediaRapidez = Math.Round(a.MediaRapidez, 2),
                    MediaDificuldade = Math.Round(a.MediaDificuldade, 2),
                    PercentualPretendeVoltar = a.TotalFeedbacks == 0
                        ? 0
                        : Math.Round(100.0 * a.TotalPretendeVoltar / a.TotalFeedbacks, 2)
                })
                .ToList();
        }

        private static FeedbackResponse ToResponse(Feedback feedback)
        {
            return new FeedbackResponse
            {
                Id = feedback.Id,
                IdDoacao = feedback.IdDoacao,
                IdCampanha = feedback.IdCampanha,
                CriadoEm = feedback.CriadoEm,
                Rapidez = feedback.Rapidez,
                Dificuldade = feedback.Dificuldade,
                PretendeVoltar = feedback.PretendeVoltar,
                Comentario = feedback.Comentario
            };
        }
    }
}
