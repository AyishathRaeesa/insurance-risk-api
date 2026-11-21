namespace InsuranceRiskApi.Domain;

public enum CoverageType
{
    Auto,
    Home,
    Health
}

public class Applicant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Location { get; set; } = string.Empty;
    public int PriorClaimsCount { get; set; }
    public CoverageType CoverageType { get; set; }
}
