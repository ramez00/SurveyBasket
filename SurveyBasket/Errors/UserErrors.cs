namespace SurveyBasket.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredrntials = 
        new("User.Invalid", "Invalid UserName/Password");

    public static readonly Error InvalidToken =
        new("User.Token", "Invalid User Token");

    public static readonly Error InvalidUser =
        new("User.NotFound", "User not found");

    public static readonly Error UserExist =
        new("User.Exist", "Email is already Exist");

    public static readonly Error EmailNotConfirmed =
        new("User.EmailNotConfirmed", "Your Email is Not Confirmed");
}
