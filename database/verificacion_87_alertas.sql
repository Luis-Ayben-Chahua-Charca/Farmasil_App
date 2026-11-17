-- ISSUE #87 - verificacion de los datos para ESP-0013 y RNF-0010.
-- SOLO LECTURA: NO INSERTA, ACTUALIZA NI BORRA REGISTROS.
-- Ejecutar contra la base de pruebas con sqlcmd o SSMS.
SET NOCOUNT ON;
DECLARE @Hoy date = CAST(GETDATE() AS date);
DECLARE @Umbral int;
DECLARE @Reglas int;

SELECT @Reglas = COUNT(*), @Umbral = MAX(umbral_meses)
FROM dbo.TBL_ALERTAS_VENCIMIENTO
WHERE estado = N'Activo';

IF @Reglas <> 1 OR @Umbral IS NULL OR @Umbral <= 0
BEGIN
    THROW 50087, 'ESP-0013 requiere exactamente una regla activa con umbral positivo.', 1;
END;

DECLARE @Limite date = DATEADD(MONTH, @Umbral, @Hoy);
PRINT 'Regla activa y fecha limite calculadas';
SELECT @Hoy AS hoy, @Umbral AS umbral_meses, @Limite AS fecha_limite;

-- Este predicado debe coincidir con el proceso 1 de RNF-0010.
SELECT L.id_lote, P.nombre AS medicamento, L.numero_lote,
       L.fecha_vencimiento, L.estado_lote, L.stock_actual,
       N'Por vencer' AS origen
FROM dbo.TBL_LOTES AS L
INNER JOIN dbo.TBL_PRODUCTOS AS P ON P.id_producto = L.id_producto
WHERE L.estado_lote = N'Disponible'
  AND L.stock_actual > 0
  AND L.fecha_vencimiento <= @Limite
ORDER BY L.fecha_vencimiento, P.nombre, L.numero_lote;

-- Los retiros registrados se muestran por separado (incluye históricos sin id_lote).
SELECT id_retiro, id_lote, nombre_producto, numero_lote,
       fecha_vencimiento, es_lote_defectuoso, N'Retirado' AS origen
FROM dbo.TBL_PRODUCTOS_RETIRADOS
ORDER BY fecha_vencimiento, nombre_producto, numero_lote;

SELECT
    (SELECT COUNT(*) FROM dbo.TBL_LOTES AS L
     WHERE L.estado_lote = N'Disponible'
       AND L.stock_actual > 0
       AND L.fecha_vencimiento <= @Limite) AS proximas_rnf0010,
    (SELECT COUNT(*) FROM dbo.TBL_PRODUCTOS_RETIRADOS) AS retiros_registrados;
-- En la carga original 002_datos_prueba.sql se esperan normalmente 2 proximas
-- (IBU-VENC-001, AMO-PROX-001) y 2 retiros, sin filtros.
-- Si hay cambios posteriores, valen los resultados calculados, no estas cifras.
