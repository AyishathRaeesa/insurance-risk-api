namespace InsuranceRiskApi.Domain;

public class Quote
{
    public int Id { get; set; }
    public int ApplicantId { get; set; }
    public Applicant? Applicant { get; set; }

    public int RiskScore { get; set; }
    public decimal Premium { get; set; }
    public DateTime DateGenerated { get; set; }
}
