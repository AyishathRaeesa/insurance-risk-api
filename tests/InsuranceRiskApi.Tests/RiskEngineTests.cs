using InsuranceRiskApi.Application.Services;
using InsuranceRiskApi.Domain;
using Xunit;

namespace InsuranceRiskApi.Tests;

public class RiskEngineTests
{
    private readonly RiskEngine _sut = new();

    [Fact]
    public void CalculateRiskScore_YoungDriverWithClaims_ReturnsHighRiskScore()
    {
        var applicant = new Applicant
        {
            Age = 22,
            Location = "Springfield",
            PriorClaimsCount = 3,
            CoverageType = CoverageType.Auto
        };

        var score = _sut.CalculateRiskScore(applicant);

        // base(20) + age(25) + location(5) + claims(30) = 80
        Assert.Equal(80, score);
    }

    [Fact]
    public void CalculateRiskScore_MiddleAgedNoClaimsLowRiskLocation_ReturnsLowRiskScore()
    {
        var applicant = new Applicant
        {
            Age = 45,
            Location = "Springfield",
            PriorClaimsCount = 0,
            CoverageType = CoverageType.Home
        };

        var score = _sut.CalculateRiskScore(applicant);

        // base(20) + age(0) + location(5) + claims(0) = 25
        Assert.Equal(25, score);
    }

    [Fact]
    public void CalculateRiskScore_HighRiskLocation_AddsExtraPoints()
    {
        var lowRisk = new Applicant { Age = 45, Location = "Springfield", PriorClaimsCount = 0, CoverageType = CoverageType.Home };
        var highRisk = new Applicant { Age = 45, Location = "Chicago", PriorClaimsCount = 0, CoverageType = CoverageType.Home };

        Assert.True(_sut.CalculateRiskScore(highRisk) > _sut.CalculateRiskScore(lowRisk));
    }

    [Fact]
    public void CalculateRiskScore_ManyPriorClaims_IsCappedAt100()
    {
        var applicant = new Applicant
        {
            Age = 20,
            Location = "Miami",
            PriorClaimsCount = 20, // would blow past 100 uncapped
            CoverageType = CoverageType.Auto
        };

        var score = _sut.CalculateRiskScore(applicant);

        Assert.Equal(100, score);
    }

    [Fact]
    public void CalculateRiskScore_NegativePriorClaims_Throws()
    {
        var applicant = new Applicant { Age = 30, Location = "X", PriorClaimsCount = -1, CoverageType = CoverageType.Auto };

        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.CalculateRiskScore(applicant));
    }

    [Theory]
    [InlineData(CoverageType.Auto, 0, 800)]
    [InlineData(CoverageType.Home, 0, 600)]
    [InlineData(CoverageType.Health, 0, 1200)]
    [InlineData(CoverageType.Auto, 100, 1600)]
    public void CalculatePremium_AppliesRiskLoadingCorrectly(CoverageType type, int riskScore, decimal expectedPremium)
    {
        var applicant = new Applicant { Age = 30, Location = "X", PriorClaimsCount = 0, CoverageType = type };

        var premium = _sut.CalculatePremium(applicant, riskScore);

        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public void CalculatePremium_InvalidRiskScore_Throws()
    {
        var applicant = new Applicant { Age = 30, Location = "X", CoverageType = CoverageType.Auto };

        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.CalculatePremium(applicant, 150));
    }
}
