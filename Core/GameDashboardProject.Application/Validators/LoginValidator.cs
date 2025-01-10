using FluentValidation;
using GameDashboardProject.Application.Constants;
using GameDashboardProject.Application.Features.Queries.Login;

namespace GameDashboardProject.Application.Validators
{
    public class LoginQueryRequestValidator : AbstractValidator<LoginQueryRequest>
    {
        public LoginQueryRequestValidator()
        {
            var genericErrorMessage = Messages.InvalidUsernameOrPassword;

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(genericErrorMessage); 

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(genericErrorMessage); 
        }
    }
}
