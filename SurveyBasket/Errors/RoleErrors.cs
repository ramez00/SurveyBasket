namespace SurveyBasket.Errors;

public static class RoleErrors
{
    public static readonly Error RoleNotFound =
        new("Role.NotFound", "Role not found with the given ID");

    public static readonly Error RoleExist =
       new("Role.Exist", "Role already exist");
}
