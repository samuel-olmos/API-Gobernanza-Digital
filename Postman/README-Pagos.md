# Documentación de Endpoints - Pagos

Base URL: {{baseUrl}}/api/pagos

## PagosController

### 1. Pagar Boleta
**Descripción:** Procesa el pago de una boleta a través de la pasarela PayLink. Este endpoint simula un pago real conectándose con la API de PayLink.

**Método:** POST

**URL:** {{baseUrl}}/api/pagos/pagar/{id}

**Path Parameters:**
- `id` (int, requerido): El ID de la boleta a pagar
  - Ejemplo: `1`

**Headers:** —

**Body:** —

**Respuestas:**

**200 OK - Pago exitoso**
```json
{
  "message": "Pago confirmado y registrado."
}
```

**400 Bad Request - Pago rechazado por pasarela**
```json
{
  "message": "El pago fue rechazado por la pasarela."
}
```

**400 Bad Request - Error de lógica de negocio**
```json
{
  "message": "La boleta ya fue pagada."
}
```
O
```json
{
  "message": "La boleta con ese ID no existe."
}
```

**500 Internal Server Error - Error de conexión**
```json
{
  "message": "Error al conectar con la pasarela.",
  "error": "Detalles del error técnico"
}
```

---

## Flujo del Proceso de Pago

1. **Buscar la boleta** en la base de datos local por ID
2. **Validar estado**: verificar que no esté ya pagada
3. **Crear payload** con los datos requeridos por PayLink:
   - TransactionId (generado automáticamente)
   - FacturaId (código de pago de la boleta)
   - Monto (monto total de la boleta)
4. **Enviar petición POST** a PayLink (`http://localhost:5105/api/payments`)
   - Header: `X-API-KEY: 14427b31c1fe49d7abed`
5. **Procesar respuesta**:
   - Si Estado = "Confirmado" → Marcar boleta como pagada
   - Si Estado != "Confirmado" → Rechazar el pago
6. **Retornar resultado** al cliente

---

## Configuración Requerida

### Variables de Environment en Postman:
- `baseUrl`: https://localhost:7054 (URL de tu API)
- `boletaId`: 1 (ID de una boleta existente)

### Requisitos Previos:
1. **PayLink API** debe estar corriendo en `http://localhost:5105`
2. **API Key válida**: `14427b31c1fe49d7abed` (hardcoded en PagoService)
3. **Boleta existente** con estado "Pendiente" o "Vencida"

---

## Ejemplo de Uso en Postman

### Request:
```
POST {{baseUrl}}/api/pagos/pagar/1
```

### Flujo Completo de Prueba:

1. **Crear o buscar una boleta:**
   ```
   GET {{baseUrl}}/api/boleta
   ```
   Copiar el `id` de una boleta con estado "Pendiente"

2. **Actualizar variable en Environment:**
   - Edita la variable `boletaId` con el ID copiado

3. **Ejecutar el pago:**
   ```
   POST {{baseUrl}}/api/pagos/pagar/{{boletaId}}
   ```

4. **Verificar resultado:**
   ```
   GET {{baseUrl}}/api/boleta/{{boletaId}}
   ```
   Buscar la boleta pagada y verificar que:
   - `estadoNombre` = "Pagada"
   - `fechaPago` tiene valor

---

## Notas Importantes

### Seguridad:
- Actualmente la API Key está hardcoded. En producción debe estar en configuración externa (appsettings.json, variables de entorno, Azure Key Vault, etc.)

### Idempotencia:
- El endpoint valida que la boleta no esté ya pagada
- Si intentas pagar dos veces, recibirás error 400

### Integración con PayLink:
- La URL de PayLink (`http://localhost:5105`) está hardcoded en `PagoService`
- Asegúrate de que PayLink esté corriendo antes de probar

### Estados de Boleta:
- **Pendiente**: puede ser pagada
- **Vencida**: puede ser pagada (con posible recargo)
- **Pagada**: no puede volver a pagarse

---

## Troubleshooting

### Error: "Error al conectar con la pasarela"
**Causa:** PayLink no está corriendo o URL incorrecta
**Solución:** 
1. Verificar que PayLink esté en `http://localhost:5105`
2. Revisar que el puerto no esté bloqueado por firewall

### Error: "La boleta con ese código de pago no existe"
**Causa:** El código de pago no existe en la BD
**Solución:** 
1. Listar boletas: `GET /api/boleta`
2. Copiar un código válido

### Error: "La boleta ya fue pagada"
**Causa:** Intentando pagar una boleta que ya tiene estado "Pagada"
**Solución:** 
1. Usar otra boleta pendiente
2. O generar nuevas boletas con `/api/boleta/generar`

### Error 401/403 en PayLink
**Causa:** API Key inválida o expirada
**Solución:** 
1. Verificar el API Key en `PagoService.cs`
2. Regenerar API Key en PayLink si es necesario
