namespace SurveyBasket.Models;

public sealed class Answer 
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int QuestionId { get; set; }
    public Question Questions { get; set; } = default!;
    public ICollection<Answer> Answers { get; set; } = [];
}
