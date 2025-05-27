namespace SurveyBasket.Errors;

public static class RoleErrors
{
    public static readonly Error RoleNotFound =
    new("Role.NotFound", "Role not found with the given ID");
}
