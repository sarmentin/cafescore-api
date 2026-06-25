using Cafescore.Application.DTOs.Auth;
using FluentValidation;

namespace Cafescore.Application.Validators;

public class RegistrarUsuarioValidator : AbstractValidator<RegistrarUsuarioDto>
{
    public RegistrarUsuarioValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(150).WithMessage("Email deve ter no máximo 150 caracteres");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres");
    }
}