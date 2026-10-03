namespace Ventas.Api.Clientes;

public record CrearClienteRequest(string Nombre, string Nif, string? Email, string Provincia);
