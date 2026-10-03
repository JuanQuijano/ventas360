# Ventas360

Aplicación de ejemplo del curso **IA aplicada al desarrollo: Copilot, Claude Code, agentes y MCP**.

Es una API de pedidos y clientes en .NET 10 sobre SQL Server. Tiene partes bien hechas y partes
heredadas con deuda técnica **a propósito**: sobre ellas se hacen las prácticas.

## Estructura

| Carpeta | Contenido |
|---|---|
| `src/Ventas.Api` | API mínima de ASP.NET Core, organizada por funcionalidad: `Clientes/` y `Pedidos/` (endpoints, peticiones y validadores) |
| `src/Ventas.Core` | Modelos, acceso a datos y servicios de negocio |
| `tests/Ventas.Tests` | Tests de xUnit |
| `db` | Scripts de SQL Server: esquema, datos, procedimientos, seguridad y migraciones |
| `etl` | Paquetes SSIS y catálogo simulado (módulo 6) |

## Puesta en marcha

Requisitos: .NET 10 SDK, SQL Server (LocalDB, Developer o contenedor) y `sqlcmd`.

```powershell
# 1. Compilar y ejecutar los tests
dotnet build
dotnet test

# 2. Crear la base de datos (por defecto en LocalDB)
.\db\instalar.ps1

# 3. Arrancar la API (http://localhost:5080)
dotnet run --project src/Ventas.Api
```

Si no usas LocalDB, configura tu cadena de conexión sin escribirla en el repositorio:

```powershell
dotnet user-secrets set "ConnectionStrings:Ventas" "Server=...;Database=Ventas360;..." --project src/Ventas.Api
```

El archivo `src/Ventas.Api/Ventas.Api.http` tiene peticiones de ejemplo para todos los endpoints.

## Ramas del curso

Cada módulo tiene una rama de partida y una de solución:

| Rama | Contenido |
|---|---|
| `m1-inicio` | Punto de partida del módulo 1 (igual que `main` al empezar el curso) |
| `m1-fin` | Solución de las prácticas E1.2 y E1.3 |

```powershell
git switch m1-inicio      # empezar las prácticas
git switch m1-fin         # ver la solución
```

## Deuda técnica conocida (intencionada)

No la corrijas por tu cuenta: cada punto es una práctica del curso.

- Un test de `CalculadoraImportesTests` falla: el IVA se trunca en lugar de redondearse (E2.3).
- `PedidoService` construye el SQL concatenando texto (riesgo de inyección), carga las líneas
  pedido a pedido (N+1) y escribe en consola en lugar de usar `ILogger` (E2.1).
- `appsettings.Production.json` contiene una contraseña escrita en el archivo (M2).
- `POST /pedidos` no valida la petición (E1.2).

## Para el formador

1. Crea un repositorio vacío en la organización de GitHub del curso (`<organizacion>/ventas360`).
2. Desde esta carpeta:

   ```powershell
   git remote add origin https://github.com/<organizacion>/ventas360.git
   git push -u origin main m1-inicio m1-fin
   ```

3. Comprueba que los alumnos tienen acceso de lectura (o que pueden hacer *fork*).
