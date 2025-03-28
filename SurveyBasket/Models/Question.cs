namespace SurveyBasket.Models;

public sealed class Question : AuditableModel
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int PollId { get; set; }
    public Poll Poll { get; set; } = default!;
    public ICollection<Answer> Answers { get; set; } = [];
}
