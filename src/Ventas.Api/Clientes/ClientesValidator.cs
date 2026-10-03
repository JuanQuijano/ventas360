using FluentValidation;

namespace Ventas.Api.Clientes;

public class ClientesValidator : AbstractValidator<CrearClienteRequest>
{
    // NIF de persona física (8 cifras y letra) o CIF de empresa (letra, 7 cifras y control)
    private const string PatronNif = "^([0-9]{8}[A-Z]|[ABCDEFGHJNPQRSUVW][0-9]{7}[0-9A-J])$";

    public ClientesValidator()
    {
        RuleFor(c => c.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede superar los 150 caracteres.");

        RuleFor(c => c.Nif)
            .NotEmpty().WithMessage("El NIF es obligatorio.")
            .Matches(PatronNif).WithMessage("El NIF no tiene un formato válido.");

        RuleFor(c => c.Email)
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .When(c => !string.IsNullOrWhiteSpace(c.Email));

        RuleFor(c => c.Provincia)
            .NotEmpty().WithMessage("La provincia es obligatoria.");
    }
}
