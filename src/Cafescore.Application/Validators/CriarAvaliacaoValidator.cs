using Cafescore.Application.DTOs.Avaliacao;
using FluentValidation;

namespace Cafescore.Application.Validators;

public class CriarAvaliacaoValidator : AbstractValidator<CriarAvaliacaoDto>
{
    public CriarAvaliacaoValidator()
    {
        RuleFor(x => x.ClinicaId)
            .NotEmpty().WithMessage("Clínica é obrigatória");

        RuleFor(x => x.Nota)
            .InclusiveBetween(1, 5).WithMessage("Nota deve ser entre 1 e 5");

        RuleFor(x => x.Comentario)
            .NotEmpty().WithMessage("Comentário é obrigatório")
            .MaximumLength(500).WithMessage("Comentário deve ter no máximo 500 caracteres");
    }
}