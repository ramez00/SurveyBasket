using Mapster;
using Microsoft.EntityFrameworkCore.Internal;
using SurveyBasket.Contracts.Polls;

namespace SurveyBasket.Mapping;

public class MappingConfigrations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Poll, PollsResponse>()
                  .Map(dest => dest.Summary, src => src.Summary);

        config.NewConfig<QuestionRequest,Question>()
            .Map(dest => dest.Answers, src => src.Answers.Select(ans => new Answer { Content = ans }));
    }
}
