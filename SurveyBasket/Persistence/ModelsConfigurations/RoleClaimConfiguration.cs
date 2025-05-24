
namespace SurveyBasket.Persistence.ModelsConfigurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        var permissions = Permissions.GetAllPermissions();
        var adminClaims = new List<IdentityRoleClaim<string>>();

        for (int i = 0; i < permissions.Length; i++)
        {
            var caliamId = i + 1; // Assuming Id is auto-incremented or set manually
            adminClaims.Add(new IdentityRoleClaim<string>
            {
                Id = caliamId,
                RoleId = DefaultRole.AdminRoleId,
                ClaimType = "permission",
                ClaimValue = permissions[i]
            });
        }

        builder.HasData(adminClaims);
    }
}
