namespace SurveyBasket.Contracts.Roles;

public record RoleDetailsResponse : RoleResponse
{
    public RoleDetailsResponse(string Id, string Name, bool IsDeleted) : base(Id, Name, IsDeleted)
    {
    }
    public IEnumerable<string> permissions { get; set; } = new List<string>();
}