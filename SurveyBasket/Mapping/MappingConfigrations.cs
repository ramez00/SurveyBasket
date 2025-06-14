﻿using Mapster;
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

        config.NewConfig<(ApplicationUser user, IList<string> roles), UserResponse>()
            .Map(dest => dest, src => src.user)
            .Map(dest => dest.Roles, src => src.roles);

        config.NewConfig<CreateUserRequest, ApplicationUser>()
           .Map(dest => dest.UserName, src => src.Email)
           .Map(dest => dest.EmailConfirmed, src => true);

        config.NewConfig<UpdateUserRequest, ApplicationUser>()
           .Map(dest => dest.UserName, src => src.Email)
           .Map(dest => dest.NormalizedUserName, src => src.Email.ToUpper());
    }
}
