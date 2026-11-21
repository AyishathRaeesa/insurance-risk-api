using System.ComponentModel.DataAnnotations;
using InsuranceRiskApi.Domain;

namespace InsuranceRiskApi.Application.Dtos;

public class CreateQuoteRequest
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(16, 100)]
    public int Age { get; set; }

    [Required, StringLength(100)]
    public string Location { get; set; } = string.Empty;

    [Range(0, 50)]
    public int PriorClaimsCount { get; set; }

    [Required]
    public CoverageType CoverageType { get; set; }
}

public class QuoteResponse
{
    public int Id { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public CoverageType CoverageType { get; set; }
    public int RiskScore { get; set; }
    public decimal Premium { get; set; }
    public DateTime DateGenerated { get; set; }
}
