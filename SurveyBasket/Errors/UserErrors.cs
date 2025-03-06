namespace SurveyBasket.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredrntials = 
        new("User.Invalid", "Invalid UserName/Password");

    public static readonly Error InvalidToken =
        new("User.Token", "Invalid User Token");

    public static readonly Error InvalidUser =
        new("User.NotFound", "User not found");
}
