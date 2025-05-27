namespace SurveyBasket.Contracts.Roles;

public record RoleDetailsResponse : RoleResponse
{
    public RoleDetailsResponse(string Id, string Name, bool IsDeleted,IEnumerable<string> permissions) : base(Id, Name, IsDeleted)
    {
        this.permissions = permissions;
    }
    public IEnumerable<string> permissions { get; set; } = new List<string>();
}