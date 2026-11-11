param(
    [string]$Base = "DB_FARMASIL",
    [string]$Servidor = "lpc:localhost\SQLEXPRESS"
)

$ErrorActionPreference = "Stop"

$DirectorioScript = Split-Path -Parent $MyInvocation.MyCommand.Path
$ScriptEsquema = Join-Path $DirectorioScript "001_esquema.sql"

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
    -b `
    -d $Base `
    -i $ScriptEsquema

if ($LASTEXITCODE -ne 0) {
    throw "Ocurrió un error al ejecutar 001_esquema.sql."
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
    -b `
    -d $Base `
    -Q $SqlVerificacion

if ($LASTEXITCODE -ne 0) {
    throw "No se pudo verificar la base de datos."
}

Write-Host ""
Write-Host "$Base recreada correctamente."