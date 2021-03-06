﻿using eshopAPI.Requests.User;
using FluentValidation;

namespace eshopAPI.Validators.Request
{
    public class UpdateUserProfileValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserProfileValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .Matches(@"^[A-Za-z]+$")
                .WithMessage("Name can only contain letters.");
            RuleFor(r => r.Surname)
                .NotEmpty()
                .Matches(@"^[A-Za-z]+$")
                .WithMessage("Surname can only contain letters.");
            RuleFor(r => r.Phone)
                .NotEmpty()
                .Matches(@"^[+]?[0-9]+$")
                .WithMessage("Phone number is not valid.");
            RuleFor(r => r.Address).SetValidator(new AddressValidator());
        }
    }
}
