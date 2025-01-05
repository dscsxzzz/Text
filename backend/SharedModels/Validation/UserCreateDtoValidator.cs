using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SharedModels.Dtos;

namespace SharedModels.Validation;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(255);
    }
}
