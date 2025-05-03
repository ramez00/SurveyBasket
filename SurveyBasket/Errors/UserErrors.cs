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

    public static readonly Error EmailAlreadyConfirmed =
               new("User.EmailAlreadyConfirmed", "Your Email is Already Confirmed");

    public static readonly Error  EmailNotFound =
           new("User.EmailSent", "Your Email is Already Sent");

    public static readonly Error EmailCofirmed =
       new("User.EmailCofirmed", "Your Email is Already Confirmed");
}
