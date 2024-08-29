﻿using FluentValidation;
using MinimalAPIsMoviesRick.DTOs;
using System.Data;

namespace MinimalAPIsMoviesRick.Validaton
{
    public class CreateActorDTOValidator : AbstractValidator<CreateActorDTO>
    {
        public CreateActorDTOValidator()
        {
            RuleFor(p=>p.Name).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(150)
                .WithMessage(ValidationUtilities.MaximunLengthMessage);

            var minimumDate = new DateTime(1900, 1, 1);
            
            RuleFor(p=>p.DateOfBirth).GreaterThanOrEqualTo(minimumDate)
                .WithMessage(ValidationUtilities.GreaterThanDate(minimumDate));
        }
    }
}
