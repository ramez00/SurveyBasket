
namespace SurveyBasket.Persistence.ModelsConfigurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasIndex(x => new { x.PollID, x.UserId }).IsUnique();
    }
}
