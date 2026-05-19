using LayeredArchitecture.Domain.Clients;
using System.Net.Http.Json;

namespace LayeredArchitecture.DataAccess.Clients;

public class HttpCreditBureauClient : ICreditBureauClient
{
    private readonly HttpClient _httpClient;

    // Injecting standard .NET HttpClient
    public HttpCreditBureauClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> GetCreditScoreAsync(Guid customerId)
    {
        // 1. Make the external HTTP call
        var response = await _httpClient.GetAsync($"https://api.fakecreditbureau.com/scores/{customerId}");

        response.EnsureSuccessStatusCode();

        // 2. Parse the result
        var data = await response.Content.ReadFromJsonAsync<CreditScoreResponse>();

        // 3. Return the data to the Application Layer
        return data?.Score ?? 0;
    }

    // Private class just for shaping the external JSON response
    private class CreditScoreResponse
    {
        public int Score { get; set; }
    }
}
