using InsuranceRiskApi.Application.Dtos;
using InsuranceRiskApi.Domain;

namespace InsuranceRiskApi.Application.Interfaces;

public interface IQuoteService
{
    Task<QuoteResponse> CreateQuoteAsync(CreateQuoteRequest request);
    Task<QuoteResponse?> GetQuoteByIdAsync(int id);
    Task<IEnumerable<QuoteResponse>> GetAllQuotesAsync(CoverageType? coverageType = null);
}
