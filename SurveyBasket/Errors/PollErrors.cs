namespace SurveyBasket.Errors;

public static class PollErrors
{
    public static readonly Error PollNotFound =
        new("Poll.NotFound", "Poll not found with the given ID");
}
