using InsuranceRiskApi.Application.Interfaces;
using InsuranceRiskApi.Domain;

namespace InsuranceRiskApi.Application.Services;

public class RiskEngine : IRiskEngine
{
    // Example high-risk locations. In a real system this would come from
    // an actuarial data source / lookup table.
    private static readonly HashSet<string> HighRiskLocations = new(StringComparer.OrdinalIgnoreCase)
    {
        "Chicago", "Los Angeles", "Miami", "Houston"
    };

    private static readonly Dictionary<CoverageType, decimal> BasePremiums = new()
    {
        { CoverageType.Auto, 800m },
        { CoverageType.Home, 600m },
        { CoverageType.Health, 1200m }
    };

    public int CalculateRiskScore(Applicant applicant)
    {
        if (applicant is null) throw new ArgumentNullException(nameof(applicant));

        int score = 20; // base score

        score += AgeRiskPoints(applicant.Age);
        score += LocationRiskPoints(applicant.Location);
        score += ClaimsHistoryRiskPoints(applicant.PriorClaimsCount);

        return Math.Clamp(score, 0, 100);
    }

    public decimal CalculatePremium(Applicant applicant, int riskScore)
    {
        if (applicant is null) throw new ArgumentNullException(nameof(applicant));
        if (riskScore is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(riskScore));

        var basePremium = BasePremiums[applicant.CoverageType];

        // Risk score linearly loads the base premium: a score of 100
        // doubles the base premium, a score of 0 keeps it at base.
        var premium = basePremium * (1 + (riskScore / 100m));

        return Math.Round(premium, 2);
    }

    private static int AgeRiskPoints(int age)
    {
        return age switch
        {
            < 25 => 25,
            >= 25 and <= 40 => 5,
            >= 41 and <= 60 => 0,
            _ => 15 // 60+
        };
    }

    private static int LocationRiskPoints(string location)
    {
        return HighRiskLocations.Contains(location?.Trim() ?? string.Empty) ? 20 : 5;
    }

    private static int ClaimsHistoryRiskPoints(int priorClaimsCount)
    {
        if (priorClaimsCount < 0) throw new ArgumentOutOfRangeException(nameof(priorClaimsCount));
        return Math.Min(priorClaimsCount * 10, 45);
    }
}
