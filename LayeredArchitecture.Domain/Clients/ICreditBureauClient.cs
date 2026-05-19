namespace LayeredArchitecture.Domain.Clients
{
    public interface ICreditBureauClient
    {
        Task<int> GetCreditScoreAsync(Guid customerId);
    }
}
