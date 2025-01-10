using MediatR;
using GameDashboardProject.Application.Abstractions.Services;
using GameDashboardProject.Domain.Identities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using GameDashboardProject.Application.Features.Queries.Login;
using GameDashboardProject.Application.Constants;
using FluentValidation;
using GameDashboardProject.Application.Abstractions;

namespace GameDashboardProject.Application.Features.Handlers
{
    public class LoginQueryHandler : IRequestHandler<LoginQueryRequest, LoginQueryResponse>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenServices _tokenServices;
        private readonly IUserSessionService _userSessionService; 
        private readonly IValidator<LoginQueryRequest> _validator;

        public LoginQueryHandler(
            UserManager<AppUser> userManager,
            ITokenServices tokenServices,
            IUserSessionService userSessionService, 
            IValidator<LoginQueryRequest> validator)
        {
            _userManager = userManager;
            _tokenServices = tokenServices;
            _userSessionService = userSessionService;
            _validator = validator;
        }

        public async Task<LoginQueryResponse> Handle(LoginQueryRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                throw new ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure("UserName", Messages.InvalidUsernameOrPassword)
                });
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                throw new ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure("Password", Messages.InvalidUsernameOrPassword)
                });
            }

            _userSessionService.InitializeSession(user.Id.ToString());

            var roles = await _userManager.GetRolesAsync(user);
            var token = await _tokenServices.CreateToken(user, roles);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginQueryResponse(tokenString, token.ValidTo, true, Messages.Loginful);
        }
    }
}
