namespace FeedbackApi.Domain.DTOs
{
    public class InfoToken
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public ERole Role { get; set; }
        public DateTime CadastradoEm { get; set; }
        public string Token { get; set; }
    }
}
