using System.Data;

namespace SurveyBasket.Contracts.Auth;

public class TokenRequestValidator : AbstractValidator<TokenRequest>
{
    public TokenRequestValidator()
    {
        RuleFor(x => x.token).NotEmpty();
        RuleFor(x => x.refreshToken).NotEmpty();
    }
}
