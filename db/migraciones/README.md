# Migraciones

Cada cambio de esquema posterior a la instalación inicial va en un script numerado:

    NNN_descripcion_corta.sql     (por ejemplo, 002_canal_pedido.sql)

Reglas:

- Un script por cambio, numerado de forma correlativa. Nunca se edita un script ya publicado.
- Los scripts deben poder ejecutarse dos veces sin error (comprobar con `COL_LENGTH`, `OBJECT_ID`, etc.).
- Las columnas nuevas en tablas con datos llevan valor por defecto o admiten NULL.
- `instalar.ps1` ejecuta todos los scripts de esta carpeta en orden alfabético.

`001` corresponde a la instalación inicial (scripts `00` a `04` de la carpeta `db`).
