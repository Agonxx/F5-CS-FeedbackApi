using FeedbackApi.Domain;
using FeedbackApi.Domain.DTOs;
using FeedbackApi.Domain.Entities;
using FeedbackApi.Domain.Interfaces.Repositories;
using FeedbackApi.Infrastructure.Data;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace FeedbackApi.Infrastructure.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly IMongoCollection<Feedback> _feedbacks;

        public FeedbackRepository(IMongoDatabase database)
        {
            _feedbacks = database.GetCollection<Feedback>(MongoConfig.FeedbacksCollection);
        }

        public async Task<bool> ExistsByDoacao(int idDoacao)
        {
            return await _feedbacks.Find(f => f.IdDoacao == idDoacao).AnyAsync();
        }

        public async Task<bool> Create(Feedback feedback)
        {
            try
            {
                await _feedbacks.InsertOneAsync(feedback);
                return true;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new Exception("Já existe um feedback para esta doação");
            }
        }

        public async Task<List<Feedback>> GetByDoador(int idDoador)
        {
            return await _feedbacks.Find(f => f.IdDoador == idDoador)
                                   .SortByDescending(f => f.CriadoEm)
                                   .ToListAsync();
        }

        public async Task<List<ResumoCampanhaAgregado>> GetResumoPorCampanha()
        {
            return await _feedbacks.AsQueryable()
                .GroupBy(f => f.IdCampanha)
                .Select(g => new ResumoCampanhaAgregado
                {
                    IdCampanha = g.Key,
                    TotalFeedbacks = g.Count(),
                    MediaRapidez = g.Average(f => f.Rapidez),
                    MediaDificuldade = g.Average(f => f.Dificuldade),
                    TotalPretendeVoltar = g.Count(f => f.PretendeVoltar == EPretendeVoltar.Sim)
                })
                .ToListAsync();
        }
    }
}
