
namespace SurveyBasket.Persistence.ModelsConfigurations;

public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName)
            .HasMaxLength(200);
        builder.Property(x => x.LastName)
            .HasMaxLength(200);
    }

}
