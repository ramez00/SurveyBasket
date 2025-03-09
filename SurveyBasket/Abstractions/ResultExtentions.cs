using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Abstractions;

public static class ResultExtentions
{
    public static ObjectResult ToProblem(this Result result, int statusCode)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Can not convert success result to a Problem");

        var problem = Results.Problem(statusCode:statusCode);
        var problemDetials = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

        problemDetials!.Extensions = new Dictionary<string, object?>
        {
            {
              "errors" , new [] { result.Error}
            }
        };

        return new ObjectResult(problemDetials);
    }
}
