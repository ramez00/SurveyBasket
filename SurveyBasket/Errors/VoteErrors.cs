namespace SurveyBasket.Errors;

public static class VoteErrors
{
    public static readonly Error VoteNotFound =
        new("Vote.NotFound", "Vote not found with the given ID");

    public static readonly Error InvalidQuestions =
       new("Vote.InvalidQuestion", "Invalid questions");

    public static readonly Error VoteDuplicated =
        new("Vote.Duplicated", "The User already Voted in this Poll");

    public static readonly Error VoteNotFoundWithPollId =
        new("Vote.NotFoundWithPollId", "There is no Question with given PollID");
}
