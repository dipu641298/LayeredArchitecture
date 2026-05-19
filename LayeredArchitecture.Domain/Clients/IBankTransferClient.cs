namespace LayeredArchitecture.Domain.Clients;

public interface IBankTransferClient
{
    Task<bool> WireFundsAsync(Guid customerId, decimal amount);
}