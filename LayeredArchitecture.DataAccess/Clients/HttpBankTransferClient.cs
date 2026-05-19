using LayeredArchitecture.Domain.Clients;
using System.Net.Http.Json;

namespace LayeredArchitecture.DataAccess.Clients;

public class HttpBankTransferClient : IBankTransferClient
{
    private readonly HttpClient _httpClient;

    public HttpBankTransferClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> WireFundsAsync(Guid customerId, decimal amount)
    {
        // Simulating an external API call to a banking partner like Stripe or Plaid
        var payload = new { CustomerId = customerId, Amount = amount };
        var response = await _httpClient.PostAsJsonAsync("/api/v1/wire-transfers", payload);

        return response.IsSuccessStatusCode;
    }
}
