using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SurveyBasket.Models;
using System.Reflection;
using System.Security.Claims;

namespace SurveyBasket.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : 
    IdentityDbContext<ApplicationUser>(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public DbSet<Poll> Polls { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<VoteAnswer> voteAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // to change all Delete Behavior From Cascade to Restricted To avoid Delete any record from database 

        var cascadFk = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(x => x.DeleteBehavior == DeleteBehavior.Cascade && !x.IsOwnership);

        foreach (var fk in cascadFk)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        //modelBuilder.ApplyConfiguration(new ModelConfigrations());
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entites = ChangeTracker.Entries<AuditableModel>();
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        foreach (var entite in entites)
        {
            if (entite.State == EntityState.Added)
                entite.Property(x => x.CreatedById).CurrentValue = userId;
            else if (entite.State == EntityState.Modified)
            {
                entite.Property(x => x.UpdatedById).CurrentValue = userId;
                entite.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
