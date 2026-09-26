using System.ComponentModel.DataAnnotations;

namespace FeedbackApi.Domain.DTOs
{
    public class FeedbackRequest
    {
        [Required] public int IdDoacao { get; set; }
        [Required] public int Rapidez { get; set; }
        [Required] public int Dificuldade { get; set; }
        [Required] public EPretendeVoltar PretendeVoltar { get; set; }
        [MaxLength(500)] public string Comentario { get; set; }
    }
}
