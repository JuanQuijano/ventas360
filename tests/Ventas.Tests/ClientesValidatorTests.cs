using Ventas.Api.Clientes;

namespace Ventas.Tests;

public class ClientesValidatorTests
{
    private readonly ClientesValidator _validador = new();

    private static CrearClienteRequest PeticionValida() =>
        new("Distribuciones Norte SL", "B12345674", "info@dnorte.es", "Asturias");

    [Fact]
    public void Validar_PeticionCorrecta_EsValida()
    {
        var resultado = _validador.Validate(PeticionValida());

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public void Validar_SinNombre_DevuelveError()
    {
        var resultado = _validador.Validate(PeticionValida() with { Nombre = "" });

        Assert.Contains(resultado.Errors, e => e.PropertyName == "Nombre");
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("12345678")]
    [InlineData("X1234567Z9")]
    public void Validar_NifIncorrecto_DevuelveError(string nif)
    {
        var resultado = _validador.Validate(PeticionValida() with { Nif = nif });

        Assert.Contains(resultado.Errors, e => e.PropertyName == "Nif");
    }

    [Fact]
    public void Validar_EmailIncorrecto_DevuelveError()
    {
        var resultado = _validador.Validate(PeticionValida() with { Email = "no-es-un-email" });

        Assert.Contains(resultado.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validar_SinEmail_EsValida()
    {
        var resultado = _validador.Validate(PeticionValida() with { Email = null });

        Assert.True(resultado.IsValid);
    }
}
