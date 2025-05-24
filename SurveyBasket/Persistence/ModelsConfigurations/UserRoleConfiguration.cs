
namespace SurveyBasket.Persistence.ModelsConfigurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(
            new IdentityUserRole<string>
            {
                UserId = DefaultUser.AdminId,
                RoleId = DefaultRole.AdminRoleId
            }
        );
    }
}
