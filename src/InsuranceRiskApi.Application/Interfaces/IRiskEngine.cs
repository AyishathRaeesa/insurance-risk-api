using InsuranceRiskApi.Domain;

namespace InsuranceRiskApi.Application.Interfaces;

public interface IRiskEngine
{
    /// <summary>
    /// Computes a risk score (0-100, higher = riskier) for the given applicant.
    /// </summary>
    int CalculateRiskScore(Applicant applicant);

    /// <summary>
    /// Computes the suggested annual premium based on coverage type and risk score.
    /// </summary>
    decimal CalculatePremium(Applicant applicant, int riskScore);
}
