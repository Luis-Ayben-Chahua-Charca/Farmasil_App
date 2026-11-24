param(
    [string]$Base = "DB_FARMASIL",
    [string]$Servidor = "lpc:localhost\SQLEXPRESS"
)

$ErrorActionPreference = "Stop"

# -I fija QUOTED_IDENTIFIER ON en la sesion de sqlcmd. El sqlcmd antiguo (ODBC) la abre en
# OFF y 002_datos_prueba.sql falla con el error 1934; el de winget ya la abre en ON.

$DirectorioScript = Split-Path -Parent $MyInvocation.MyCommand.Path
$ScriptEsquema = Join-Path $DirectorioScript "001_esquema.sql"
$ScriptDatos = Join-Path $DirectorioScript "002_datos_prueba.sql"

if (-not (Test-Path $ScriptEsquema)) {
    throw "No se encontró el archivo: $ScriptEsquema"
}

Write-Host "Servidor: $Servidor"
Write-Host "Base: $Base"

Write-Host ""
Write-Host "Eliminando la base si ya existe..."

$SqlRecrear = @"
IF DB_ID(N'$Base') IS NOT NULL
BEGIN
    ALTER DATABASE [$Base] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$Base];
END;

CREATE DATABASE [$Base]
COLLATE Modern_Spanish_CI_AS;
"@

sqlcmd `
    -S $Servidor `
    -E `
    -C `
    -I `
    -b `
    -Q $SqlRecrear

if ($LASTEXITCODE -ne 0) {
    throw "No se pudo crear la base de datos '$Base'."
}

Write-Host "Base creada correctamente."

Write-Host ""
Write-Host "Ejecutando 001_esquema.sql..."

sqlcmd `
    -S $Servidor `
    -E `
    -C `
    -I `
    -b `
    -d $Base `
    -i $ScriptEsquema

if ($LASTEXITCODE -ne 0) {
    throw "Ocurrió un error al ejecutar 001_esquema.sql."
}

Write-Host ""
Write-Host "Ejecutando 002_datos_prueba.sql..."

sqlcmd `
    -S $Servidor `
    -E `
    -C `
    -I `
    -b `
    -d $Base `
    -i $ScriptDatos

if ($LASTEXITCODE -ne 0) {
    throw "Ocurrió un error al ejecutar 002_datos_prueba.sql."
}

Write-Host ""
Write-Host "Verificando objetos creados..."

$SqlVerificacion = @"
SELECT
    (SELECT COUNT(*)
     FROM sys.tables
     WHERE is_ms_shipped = 0) AS tablas,

    (SELECT COUNT(*)
     FROM sys.views
     WHERE is_ms_shipped = 0) AS vistas;
"@

sqlcmd `
    -S $Servidor `
    -E `
    -C `
    -I `
    -b `
    -d $Base `
    -Q $SqlVerificacion

if ($LASTEXITCODE -ne 0) {
    throw "No se pudo verificar la base de datos."
}

Write-Host ""
Write-Host "$Base recreada correctamente."
