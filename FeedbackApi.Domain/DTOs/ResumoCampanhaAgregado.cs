namespace FeedbackApi.Domain.DTOs
{
    public class ResumoCampanhaAgregado
    {
        public int IdCampanha { get; set; }
        public int TotalFeedbacks { get; set; }
        public double MediaRapidez { get; set; }
        public double MediaDificuldade { get; set; }
        public int TotalPretendeVoltar { get; set; }
    }
}
