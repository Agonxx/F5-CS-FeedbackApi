using System.Net.Http.Headers;
using System.Net.Http.Json;
using FeedbackApi.Domain.DTOs;
using FeedbackApi.Domain.Interfaces.Services;

namespace FeedbackApi.Infrastructure.Services
{
    public class DoacaoClient : IDoacaoClient
    {
        private readonly HttpClient _http;
        private readonly InfoToken _infoToken;

        public DoacaoClient(HttpClient http, InfoToken infoToken)
        {
            _http = http;
            _infoToken = infoToken;
        }

        public async Task<List<DoacaoInfo>> GetMinhasDoacoes()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Doacao/MinhasDoacoes");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _infoToken.Token);

            using var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Não foi possível consultar as doações na CampanhasApi");

            return await response.Content.ReadFromJsonAsync<List<DoacaoInfo>>() ?? new List<DoacaoInfo>();
        }
    }
}
