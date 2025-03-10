namespace SurveyBasket.Errors;

public static class PollErrors
{
    public static readonly Error PollNotFound =
        new("Poll.NotFound", "Poll not found with the given ID");

    public static readonly Error PollDuplicated =
        new("Poll.Duplicated", "There is another Poll Already Exists with Title");
}
