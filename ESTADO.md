# Estado del trabajo — Ventas360

Fichero para coordinar el trabajo entre sesiones y máquinas. Actualízalo al terminar
cada tarea y súbelo junto con el cambio. Antes de empezar, haz `git pull` y léelo.

Última actualización: 2026-11-10 · rama `e12-claude`

## En curso

_Nada en curso._

## Pendiente

1. **Redondeo del IVA en `CalculadoraImportes`.** El test
   `CalcularIva_ConDecimales_RedondeaAlCentimoMasProximo` espera 2,31 y la calculadora
   devuelve 2,30. Falla a propósito: no tocarlo hasta que se decida arreglarlo.
2. **Inyección SQL en `PedidoService`.** Las consultas se montan concatenando texto
   (por ejemplo, `textoCliente` y `estado` en `GET /pedidos`). Pasar a parámetros.
3. **Productos inexistentes en `PedidoService.Crear`.** Si un `ProductoId` no existe, la
   línea se descarta en silencio. Validarlo o devolver un error.
4. **Carpeta `Curso/`.** Está sin seguimiento en Git; decidir si se versiona o se ignora.

## Hecho

- **2026-11-04 · Validación de `CrearPedidoRequest`** (commit `c7f5e7f`).
  `PedidosValidator` (FluentValidation) en `src/Ventas.Api/Pedidos/`, registrado en
  `Program.cs`. Reglas: al menos una línea; cantidad de cada línea entre 1 y 999; el
  cliente debe existir y estar activo (`IClienteRepositorio.ExisteActivoAsync`).
  `POST /pedidos` devuelve 400 con `Results.ValidationProblem` y mensajes en español.
  Tests en `tests/Ventas.Tests/PedidosValidatorTests.cs` (10 casos, con
  `ClienteRepositorioFalso`).

## Estado de los tests

23 tests en `Ventas.Tests`: pasan 22. Solo falla
`CalcularIva_ConDecimales_RedondeaAlCentimoMasProximo` (pendiente 1, esperado).
