﻿namespace SurveyBasket.Models;

public sealed class VoteAnswer
{
    public int Id { get; set; }
    public int voteId { get; set; }
    public int QuestionId { get; set; }
    public int AnswerId { get; set; }

    public Vote vote { get; set; } = default!;
    public Question Question { get; set; } = default!;
    public Answer Answer { get; set; } = default!;
}
