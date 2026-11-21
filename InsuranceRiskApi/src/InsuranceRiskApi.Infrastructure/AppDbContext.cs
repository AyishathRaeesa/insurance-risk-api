using InsuranceRiskApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace InsuranceRiskApi.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<Quote> Quotes => Set<Quote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Quote>()
            .HasOne(q => q.Applicant)
            .WithMany()
            .HasForeignKey(q => q.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Quote>()
            .Property(q => q.Premium)
            .HasPrecision(10, 2);
    }
}
