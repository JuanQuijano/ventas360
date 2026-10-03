<#
.SYNOPSIS
  Crea la base de datos Ventas360 desde cero y aplica las migraciones.
.EXAMPLE
  .\db\instalar.ps1
  .\db\instalar.ps1 -Servidor "localhost,1433" -Usuario sa
#>
param(
    [string] $Servidor = "(localdb)\MSSQLLocalDB",
    [string] $Usuario
)

$ErrorActionPreference = "Stop"
$carpeta = $PSScriptRoot

if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
    throw "No se encuentra sqlcmd. Instálalo con: winget install sqlcmd"
}

$auth = @("-E")
if ($Usuario) {
    $clave = Read-Host "Contraseña de $Usuario" -AsSecureString
    $env:SQLCMDPASSWORD = [System.Net.NetworkCredential]::new("", $clave).Password
    $auth = @("-U", $Usuario)
}

$scripts = @(Get-ChildItem $carpeta -Filter "0*.sql" | Sort-Object Name) +
           @(Get-ChildItem (Join-Path $carpeta "migraciones") -Filter "*.sql" -ErrorAction SilentlyContinue | Sort-Object Name)

foreach ($s in $scripts) {
    Write-Host "Ejecutando $($s.Name)..." -ForegroundColor Cyan
    & sqlcmd -S $Servidor @auth -b -f 65001 -i $s.FullName
    if ($LASTEXITCODE -ne 0) { throw "Error en $($s.Name)" }
}

Remove-Item Env:\SQLCMDPASSWORD -ErrorAction SilentlyContinue
Write-Host "Base de datos Ventas360 lista en $Servidor" -ForegroundColor Green
