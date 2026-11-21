using InsuranceRiskApi.Application.Dtos;
using InsuranceRiskApi.Application.Interfaces;
using InsuranceRiskApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace InsuranceRiskApi.Infrastructure;

public class QuoteService : IQuoteService
{
    private readonly AppDbContext _db;
    private readonly IRiskEngine _riskEngine;

    public QuoteService(AppDbContext db, IRiskEngine riskEngine)
    {
        _db = db;
        _riskEngine = riskEngine;
    }

    public async Task<QuoteResponse> CreateQuoteAsync(CreateQuoteRequest request)
    {
        var applicant = new Applicant
        {
            Name = request.Name,
            Age = request.Age,
            Location = request.Location,
            PriorClaimsCount = request.PriorClaimsCount,
            CoverageType = request.CoverageType
        };

        var riskScore = _riskEngine.CalculateRiskScore(applicant);
        var premium = _riskEngine.CalculatePremium(applicant, riskScore);

        var quote = new Quote
        {
            RiskScore = riskScore,
            Premium = premium,
            DateGenerated = DateTime.UtcNow,
            Applicant = applicant
        };

        _db.Applicants.Add(applicant);
        _db.Quotes.Add(quote);
        await _db.SaveChangesAsync();

        return ToResponse(quote, applicant);
    }

    public async Task<QuoteResponse?> GetQuoteByIdAsync(int id)
    {
        var quote = await _db.Quotes
            .Include(q => q.Applicant)
            .FirstOrDefaultAsync(q => q.Id == id);

        return quote is null ? null : ToResponse(quote, quote.Applicant!);
    }

    public async Task<IEnumerable<QuoteResponse>> GetAllQuotesAsync(CoverageType? coverageType = null)
    {
        var query = _db.Quotes.Include(q => q.Applicant).AsQueryable();

        if (coverageType is not null)
        {
            query = query.Where(q => q.Applicant!.CoverageType == coverageType);
        }

        var quotes = await query.OrderByDescending(q => q.DateGenerated).ToListAsync();
        return quotes.Select(q => ToResponse(q, q.Applicant!));
    }

    private static QuoteResponse ToResponse(Quote quote, Applicant applicant) => new()
    {
        Id = quote.Id,
        ApplicantName = applicant.Name,
        CoverageType = applicant.CoverageType,
        RiskScore = quote.RiskScore,
        Premium = quote.Premium,
        DateGenerated = quote.DateGenerated
    };
}
