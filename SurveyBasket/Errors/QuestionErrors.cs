namespace SurveyBasket.Errors;

public static class QuestionErrors
{
    public static readonly Error QuestionNotFound =
        new("Question.NotFound", "Question not found with the given ID");

    public static readonly Error QuestionDuplicated =
        new("Question.Duplicated", "There is another Question Already Exists with Title");

    public static readonly Error QuestionNotFoundWithPollId =
        new("Question.NotFoundWithPollId", "There is no Question with given PollID");
}
