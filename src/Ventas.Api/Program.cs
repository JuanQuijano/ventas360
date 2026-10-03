using FluentValidation;
using Ventas.Api.Clientes;
using Ventas.Api.Pedidos;
using Ventas.Core.Datos;
using Ventas.Core.Servicios;

var builder = WebApplication.CreateBuilder(args);

var cadenaConexion = builder.Configuration.GetConnectionString("Ventas")
    ?? throw new InvalidOperationException(
        "Falta la cadena de conexión 'Ventas'. Revisa appsettings.Development.json o configúrala con dotnet user-secrets.");

builder.Services.AddSingleton(new ConexionBd(cadenaConexion));
builder.Services.AddSingleton<CalculadoraImportes>();
builder.Services.AddScoped<IClienteRepositorio, SqlClienteRepositorio>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IValidator<CrearClienteRequest>, ClientesValidator>();
builder.Services.AddScoped<IValidator<CrearPedidoRequest>, CrearPedidoValidator>();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/", () => Results.Ok(new { servicio = "Ventas360 API", version = "1.0" }));
app.MapClientes();
app.MapPedidos();

app.Run();

// Necesario para los tests de integración con WebApplicationFactory
public partial class Program { }
