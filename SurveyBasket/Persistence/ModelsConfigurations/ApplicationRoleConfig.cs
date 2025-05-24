
namespace SurveyBasket.Persistence.ModelsConfigurations;

public class ApplicationRoleConfig : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {

        builder.HasData([
            new ApplicationRole
            {
                Id = DefaultRole.AdminRoleId,
                Name = DefaultRole.Admin,
                NormalizedName = DefaultRole.Admin.ToUpper(),
                ConcurrencyStamp = DefaultRole.AdminRoleConcurrencyStamp
            },
            new ApplicationRole
            {
                Id = DefaultRole.MemberRoleId,
                Name = DefaultRole.Member,
                NormalizedName = DefaultRole.Member.ToUpper(),
                ConcurrencyStamp = DefaultRole.MemberRoleConcurrencyStamp,
                IsDefault = true
            }
        ]);
    }
}