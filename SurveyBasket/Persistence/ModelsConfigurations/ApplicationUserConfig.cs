
namespace SurveyBasket.Persistence.ModelsConfigurations;

public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .OwnsMany(x => x.RefreshTokens)
            .ToTable("RefreshTokens")   // To Change Table Name 
            .WithOwner()  // fully dependent on the parent entity. => USERs
            .HasForeignKey("UserId"); // To Change Forign key Name

        builder.Property(x => x.FirstName).HasMaxLength(200);
        builder.Property(x => x.LastName).HasMaxLength(200);
    }

}
