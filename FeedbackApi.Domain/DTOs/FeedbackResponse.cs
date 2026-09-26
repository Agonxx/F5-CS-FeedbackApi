namespace FeedbackApi.Domain.DTOs
{
    public class FeedbackResponse
    {
        public string Id { get; set; }
        public int IdDoacao { get; set; }
        public int IdCampanha { get; set; }
        public DateTime CriadoEm { get; set; }
        public int Rapidez { get; set; }
        public int Dificuldade { get; set; }
        public EPretendeVoltar PretendeVoltar { get; set; }
        public string Comentario { get; set; }
    }
}
