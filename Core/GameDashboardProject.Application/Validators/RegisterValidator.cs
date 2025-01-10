using FluentValidation;
using GameDashboardProject.Application.Constants;
using GameDashboardProject.Application.Features.Commands;

namespace GameDashboardProject.Application.Validators
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            // Kullanıcı Adı Validasyonu
            RuleFor(x => x.UserName)
                .Cascade(CascadeMode.Continue) 
                .NotEmpty().WithMessage(Messages.UsernameRequired) 
                .Length(3, 20).WithMessage(Messages.UsernameTooShort) 
                .Matches("^[a-zA-Z0-9]*$").WithMessage(Messages.InvalidUserName) 
                .Must(userName => !userName.Contains("admin"))
                    .WithMessage(Messages.InvalidUsernameAdmin); 
            // Email Validasyonu
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Continue)
                .NotEmpty().WithMessage(Messages.EmailRequired) 
                .EmailAddress().WithMessage(Messages.InvalidEmailFormat); 

            // Şifre Validasyonu
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Continue)
                .NotEmpty().WithMessage(Messages.PasswordRequired) 
                .MinimumLength(6).WithMessage(Messages.PasswordTooShort) 
                .Matches("[A-Z]").WithMessage(Messages.PasswordTooWeak) 
                .Matches("[a-z]").WithMessage(Messages.PasswordTooWeak) 
                .Matches("[0-9]").WithMessage(Messages.PasswordTooWeak) 
                .Matches("[^a-zA-Z0-9]").WithMessage(Messages.PasswordTooWeak);

            // Şifre Doğrulama
            RuleFor(x => x.ConfirmPassword)
                .Cascade(CascadeMode.Continue)
                .NotEmpty().WithMessage(Messages.ConfirmPasswordRequired) 
                .Equal(x => x.Password).WithMessage(Messages.PasswordsDoNotMatch); 
        }
    }

}
