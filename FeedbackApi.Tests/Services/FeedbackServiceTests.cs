using FeedbackApi.Application.Services;
using FeedbackApi.Domain;
using FeedbackApi.Domain.DTOs;
using FeedbackApi.Domain.Entities;
using FeedbackApi.Domain.Interfaces.Repositories;
using FeedbackApi.Domain.Interfaces.Services;
using Moq;

namespace FeedbackApi.Tests.Services
{
    public class FeedbackServiceTests
    {
        private readonly Mock<IFeedbackRepository> _repoMock;
        private readonly Mock<IDoacaoClient> _clientMock;
        private readonly InfoToken _infoToken;
        private readonly FeedbackService _service;

        public FeedbackServiceTests()
        {
            _repoMock = new Mock<IFeedbackRepository>();
            _clientMock = new Mock<IDoacaoClient>();
            _infoToken = new InfoToken { Id = 7, Role = ERole.Doador };
            _service = new FeedbackService(_repoMock.Object, _clientMock.Object, _infoToken);
        }

        private static FeedbackRequest RequestValido(int idDoacao = 10) => new()
        {
            IdDoacao = idDoacao,
            Rapidez = 4,
            Dificuldade = 2,
            PretendeVoltar = EPretendeVoltar.Sim,
            Comentario = "Foi simples"
        };

        private void DoadorTemDoacao(int idDoacao, int idCampanha)
        {
            _clientMock.Setup(c => c.GetMinhasDoacoes())
                       .ReturnsAsync(new List<DoacaoInfo> { new() { Id = idDoacao, IdCampanha = idCampanha } });
        }

        [Fact]
        public async Task EnviarAsync_DeveCriarFeedback_QuandoDoacaoEDoDoador()
        {
            DoadorTemDoacao(10, 3);
            _repoMock.Setup(r => r.ExistsByDoacao(10)).ReturnsAsync(false);

            var resultado = await _service.EnviarAsync(RequestValido());

            Assert.Equal(10, resultado.IdDoacao);
            Assert.Equal(3, resultado.IdCampanha);
            _repoMock.Verify(r => r.Create(It.Is<Feedback>(f =>
                f.IdDoacao == 10 && f.IdCampanha == 3 && f.IdDoador == 7 &&
                f.Rapidez == 4 && f.PretendeVoltar == EPretendeVoltar.Sim)), Times.Once);
        }

        [Fact]
        public async Task EnviarAsync_DeveLancarExcecao_QuandoDoacaoNaoEDoDoador()
        {
            DoadorTemDoacao(10, 3);

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.EnviarAsync(RequestValido(idDoacao: 99)));

            Assert.Equal("Doação não encontrada para o doador logado", ex.Message);
            _repoMock.Verify(r => r.Create(It.IsAny<Feedback>()), Times.Never);
        }

        [Fact]
        public async Task EnviarAsync_DeveLancarExcecao_QuandoJaExisteFeedbackDaDoacao()
        {
            DoadorTemDoacao(10, 3);
            _repoMock.Setup(r => r.ExistsByDoacao(10)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.EnviarAsync(RequestValido()));

            Assert.Equal("Já existe um feedback para esta doação", ex.Message);
            _repoMock.Verify(r => r.Create(It.IsAny<Feedback>()), Times.Never);
        }

        [Theory]
        [InlineData(0, 3, "A nota de rapidez deve ser de 1 a 5")]
        [InlineData(6, 3, "A nota de rapidez deve ser de 1 a 5")]
        [InlineData(3, 0, "A nota de dificuldade deve ser de 1 a 5")]
        [InlineData(3, 6, "A nota de dificuldade deve ser de 1 a 5")]
        public async Task EnviarAsync_DeveLancarExcecao_QuandoNotaForaDoIntervalo(int rapidez, int dificuldade, string mensagem)
        {
            var request = RequestValido();
            request.Rapidez = rapidez;
            request.Dificuldade = dificuldade;

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.EnviarAsync(request));

            Assert.Equal(mensagem, ex.Message);
            _clientMock.Verify(c => c.GetMinhasDoacoes(), Times.Never);
        }

        [Fact]
        public async Task EnviarAsync_DeveLancarExcecao_QuandoPretendeVoltarInvalido()
        {
            var request = RequestValido();
            request.PretendeVoltar = (EPretendeVoltar)99;

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.EnviarAsync(request));

            Assert.Equal("Informe se pretende voltar a doar: Sim, Talvez ou Nao", ex.Message);
        }

        [Fact]
        public async Task GetMeusAsync_DeveBuscarPeloDoadorLogado()
        {
            _repoMock.Setup(r => r.GetByDoador(7)).ReturnsAsync(new List<Feedback>
            {
                new() { Id = "a", IdDoacao = 1, IdCampanha = 2, IdDoador = 7 }
            });

            var resultado = await _service.GetMeusAsync();

            Assert.Single(resultado);
            Assert.Equal(1, resultado[0].IdDoacao);
        }

        [Fact]
        public async Task GetResumoAsync_DeveCalcularPercentualEArredondar()
        {
            _repoMock.Setup(r => r.GetResumoPorCampanha()).ReturnsAsync(new List<ResumoCampanhaAgregado>
            {
                new() { IdCampanha = 2, TotalFeedbacks = 3, MediaRapidez = 4.3333333, MediaDificuldade = 2, TotalPretendeVoltar = 1 },
                new() { IdCampanha = 1, TotalFeedbacks = 4, MediaRapidez = 5, MediaDificuldade = 1.5, TotalPretendeVoltar = 4 }
            });

            var resultado = await _service.GetResumoAsync();

            Assert.Equal(new[] { 1, 2 }, resultado.Select(r => r.IdCampanha));
            Assert.Equal(100, resultado[0].PercentualPretendeVoltar);
            Assert.Equal(33.33, resultado[1].PercentualPretendeVoltar);
            Assert.Equal(4.33, resultado[1].MediaRapidez);
        }
    }
}
