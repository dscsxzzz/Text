using System;
using FluentValidation;
using SharedModels.Requests;

namespace SharedModels.Validation;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(255).WithMessage("Password must be less than 255 characters long.");
    }
}
