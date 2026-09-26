namespace FeedbackApi.Domain.Entities
{
    public class Feedback
    {
        public string Id { get; set; }
        public int IdDoacao { get; set; }
        public int IdCampanha { get; set; }
        public int IdDoador { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public int Rapidez { get; set; }
        public int Dificuldade { get; set; }
        public EPretendeVoltar PretendeVoltar { get; set; }
        public string Comentario { get; set; }
    }
}
