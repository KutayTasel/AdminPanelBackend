using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using GameDashboardProject.Application.Constants;
using GameDashboardProject.Domain.Identities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GameDashboardProject.Application.Features.Commands
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IValidator<RegisterCommand> _validator; 

        public RegisterCommandHandler(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IValidator<RegisterCommand> validator) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _validator = validator;
        }

        public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new InvalidOperationException($"Validation failed: {errors}");
            }

            var existingUserByName = await _userManager.FindByNameAsync(request.UserName);
            if (existingUserByName != null)
            {
                throw new InvalidOperationException(Messages.DuplicateUserName);
            }

            var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingUserByEmail != null)
            {
                throw new InvalidOperationException(Messages.DuplicateEmail);
            }

            var appUser = new AppUser
            {
                UserName = request.UserName,
                Email = request.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createResult = await _userManager.CreateAsync(appUser, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"{Messages.RegistrationFailed} {errors}");
            }

            var adminRole = "Admin";
            if (!await _roleManager.RoleExistsAsync(adminRole))
            {
                await _roleManager.CreateAsync(new AppRole { Name = adminRole });
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(appUser, adminRole);
            if (!addToRoleResult.Succeeded)
            {
                var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to assign Admin role: {errors}");
            }

            return Unit.Value;
        }

    }
}




