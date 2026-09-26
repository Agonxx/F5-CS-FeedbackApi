namespace FeedbackApi.Domain.DTOs
{
    public class ResumoCampanhaResponse
    {
        public int IdCampanha { get; set; }
        public int TotalFeedbacks { get; set; }
        public double MediaRapidez { get; set; }
        public double MediaDificuldade { get; set; }
        public double PercentualPretendeVoltar { get; set; }
    }
}
