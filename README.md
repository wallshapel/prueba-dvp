# 🧾 Guía de Implementación y Documentación del Proyecto de Facturación Electrónica

**Autor:** ABEL CAMILO YI MARTÍNEZ
**Repositorio:** [https://github.com/wallshapel/prueba-dvp](https://github.com/wallshapel/prueba-dvp)  
**Empresa solicitante:** Double V Partners NYX  
**Fecha:** Noviembre 2025  

---

## 🌐 Contexto del Problema

La empresa **FactuMarket S.A.** busca modernizar su sistema de facturación electrónica. El sistema anterior era monolítico, lento y con poca trazabilidad. Se requiere ahora una arquitectura moderna basada en **microservicios**, que permita:

- Registrar y gestionar clientes.
- Emitir facturas electrónicas.
- Registrar eventos de auditoría.
- Mantener independencia, escalabilidad y bajo acoplamiento entre servicios.

Con esto, se busca resolver los principales problemas identificados:

1. Demoras en la emisión de facturas.  
2. Duplicación de información.  
3. Falta de trazabilidad.  
4. Escasa flexibilidad tecnológica.

---

## ⚙️ Arquitectura General del Sistema

El proyecto está compuesto por **dos microservicios** principales:

| Microservicio | Lenguaje | Base de Datos | Responsabilidad |
|----------------|-----------|----------------|------------------|
| **BillingService** | .NET 8 | Oracle | Gestión de clientes y facturas |
| **AuditService** | Ruby on Rails | MongoDB | Registro de eventos de auditoría |

Ambos se ejecutan de forma **independiente**, pero se comunican vía HTTP REST.  
La orquestación y red interna se manejan con **Docker Compose**.

### 🔄 Flujo de comunicación

1. El usuario realiza una petición al **BillingService** (por ejemplo, crear cliente o factura).  
2. El BillingService realiza la operación en su base de datos Oracle.  
3. Inmediatamente, el BillingService notifica al **AuditService** para registrar el evento en MongoDB (creación, consulta, error, etc.).  
4. El AuditService almacena el evento y permite su consulta posterior.

### 🧱 Persistencia de datos
- **Oracle:** Datos transaccionales (Clientes, Facturas).  
- **MongoDB:** Logs de auditoría (acciones, fechas, detalles).

### 💡 Principios aplicados

- **Microservicios:** independencia total, despliegue autónomo.  
- **Clean Architecture:** separación clara entre dominio, aplicación, infraestructura y capa externa.  
- **MVC:** en Rails y en la capa WebAPI del microservicio .NET.

---

## 🧩 Microservicio 1: AuditService (Ruby on Rails + MongoDB)

### 📂 Estructura principal del proyecto

```
audit_service/
├── app/
│   ├── controllers/
│   ├── dtos/
│   ├── errors/
│   ├── factories/
│   ├── jobs/
│   ├── mailers/
│   ├── models/
│   ├── repositories/
│   │   ├── services/
│   │   └── validators/
│   └── views/
├── config/
├── db/
├── Dockerfile
└── Gemfile
```

### 🧠 Descripción técnica

Este servicio implementa **una arquitectura MVC moderna**, reforzada con principios SOLID y DDD simplificado:

- **DTOs:** objetos que estructuran la data recibida o devuelta.
- **Repositories:** encapsulan la lógica de acceso a datos (MongoDB).
- **Validators:** garantizan integridad de los datos antes de persistir.
- **Services:** definen la lógica de negocio (registro y consulta de auditoría).
- **Serializadores:** estandarizan las respuestas HTTP.

Todo el manejo de errores está centralizado mediante un **interceptor global**, garantizando respuestas consistentes y legibles.

### 🧾 Endpoints principales

| Verbo | Endpoint | Descripción |
|--------|-----------|---------------|
| `POST` | `/audit_events` | Registra un evento de auditoría |
| `GET` | `/audit_events/by_entity/:entity_id` | Consulta eventos relacionados con una factura o cliente |

#### 🔍 Ejemplo de consulta por ID de factura

```
GET http://localhost:3000/audit_events/by_entity/6995e6db-9df2-4f55-9d0f-564cfd6ff886
```

**Respuesta:**
```json
{
  "status": 200,
  "success": true,
  "message": "Success",
  "timestamp": "2025-11-03T00:10:43Z",
  "data": [
    {
      "Id": "6907f0b18dbcf32e63145b15",
      "entityType": "Invoice",
      "entityId": "6995e6db-9df2-4f55-9d0f-564cfd6ff886",
      "action": "read by id",
      "performedBy": "Admin",
      "details": { "number": "INV-666" },
      "createdAt": "2025-11-03T00:00:49.685Z"
    },
    {
      "Id": "6907f00b8dbcf32e63145b14",
      "entityType": "Invoice",
      "entityId": "6995e6db-9df2-4f55-9d0f-564cfd6ff886",
      "action": "creation",
      "performedBy": "Admin",
      "details": { "number": "INV-666", "total": 450.75 },
      "createdAt": "2025-11-02T23:58:03.481Z"
    }
  ]
}
```

---

## 🧩 Microservicio 2: BillingService (.NET 8 + Oracle)

### 📂 Estructura principal del proyecto

```
BillingService.sln
├── BillingService.Domain/
├── BillingService.Application/
├── BillingService.Infrastructure/
├── BillingService.Domain.Tests/
└── BillingService.WebApi/
```

### 🧠 Descripción técnica

Este servicio implementa una **arquitectura hexagonal (Clean Architecture)**, separada en cuatro capas:

#### 1. 🧩 **Domain**
Contiene las entidades puras (`Customer`, `Invoice`), excepciones de dominio y contratos.  
Sin dependencias hacia capas externas.

#### 2. ⚙️ **Application**
Contiene la lógica de negocio, DTOs, validadores y servicios.  
Define los contratos que deben cumplir las capas inferiores.

#### 3. 🗄️ **Infrastructure**
Implementa los repositorios (Oracle con EF Core), clientes HTTP (para AuditService), y los contextos de persistencia.

#### 4. 🌍 **WebApi**
Contiene los controladores, middlewares, pipeline de configuración, inyección de dependencias, validaciones y variables de entorno.  

### ⚙️ Configuración del entorno

- Base de datos Oracle: `gvenzl/oracle-xe`
- Conexión configurada por variables de entorno (en `docker-compose.yml`)
- Swagger habilitado en: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

## 🚀 Ejecución del proyecto

1. Clonar el repositorio:
```bash
git clone https://github.com/wallshapel/prueba-dvp.git
cd prueba-dvp
```

2. Levantar las bases de datos y servicios iniciales:
```bash
docker compose up oracle-db-dvp mongodb-dvp audit-service-dvp
```

3. Esperar en los logs hasta que aparezca el siguiente mensaje:
```
                              ######################### 
oracle-db-dvp      | DATABASE IS READY TO USE!  
oracle-db-dvp      | #########################
```

⚠️ `Importante antes de seguir:` El comando siguiente del paso 4, creará el contenedor del microservicio de facturación y clientes, y generará las migraciones. Posteriormente, si desea, podrá verlo desde un cliente como DBeaver con los datos de conexión siguientes:
- Usuario/Esquema: `dvp`
- contraseña: `TuContrasena123*`
- Base de datos/Servicio: `XEPDB1`

4. Una vez aparezca el mensaje, abrir **otra consola** en la misma ruta del proyecto y ejecutar:
```bash
docker compose up billing-service-dvp
```

Los contenedores levantarán:
- Oracle XE
- MongoDB
- AuditService (Rails)
- BillingService (.NET)

Por defecto en auditoría el usuario es **Admin**, ya que el sistema no posee autenticación de usuarios ni gestión de roles.

5. Acceder a los servicios:
   - Swagger (.NET): [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)
   - Rails Audit API: [http://localhost:3000](http://localhost:3000) # Esta url es solo la base y no mostrará nada en un navegador. Ver Ejemplo de consulta por ID de factura para obtener el endpoint completo

---

## 📡 Endpoints del BillingService (.NET)

### 👤 Crear cliente
```http
POST http://localhost:8080/api/Customers
```
**Body:**
```json
{
  "idType": "CC",
  "document": "1045667805",
  "legalName": "Legato Bluesummers",
  "email": "legato@example.com",
  "address": "Calle 123 #45-67",
  "phone": "3003586120"
}
```
**Respuesta:** 201 Created ✅

---

### 📋 Obtener todos los clientes
```http
GET http://localhost:8080/api/Customers
```
**Respuesta:**
```json
{
  "status": 200,
  "success": true,
  "message": "Success",
  "data": [ { ...clientes... } ]
}
```

---

### 🔍 Obtener cliente por ID
```http
GET http://localhost:8080/api/Customers/{id}
```
**Respuesta:**
```json
{
  "status": 200,
  "success": true,
  "message": "Success",
  "data": { ...cliente... }
}
```

---

### 💰 Crear factura
```http
POST http://localhost:8080/api/Invoices
```
**Body:**
```json
{
  "customerId": "596ddb0c-48bf-4a0e-9f5a-dea81c8896ae",
  "number": "INV-666",
  "totalAmount": 450.75,
  "currency": "USD",
  "issueDate": "2025-10-31T00:00:00Z",
  "notes": "first invoice"
}
```
**Respuesta:** 201 Created ✅

---

### 🧾 Obtener factura por ID
```http
GET http://localhost:8080/api/Invoices/{id}
```
**Respuesta:** 200 OK

---

### 📅 Buscar facturas por rango de fechas
```http
GET http://localhost:8080/api/Invoices/range?from=2025-10-30&to=2025-10-31
```
**Respuesta:**
```json
{
  "status": 200,
  "success": true,
  "message": "Success",
  "data": [ { ...factura... } ]
}
```

---

## ⚠️ Algunas de las respuestas de error controladas

| Situación | Ejemplo de Respuesta |
|-------------|----------------------|
| Crear cliente sin documento | `{ "status": 400, "message": "Validation failed: document is required." }` |
| Buscar cliente inexistente | `{ "status": 404, "message": "Customer with ID 'xxx' not found." }` |
| Crear factura con número duplicado | `{ "status": 400, "message": "Invoice with same number already exists." }` |
| También hay respuestas para malformaciones de id |

Todas las respuestas del sistema siguen un formato homogéneo con las claves:
```
status, success, message, timestamp, data
```

---

## 🧩 Diagrama de Arquitectura (texto descriptivo)

```
                ┌──────────────────────────┐
                │      Frontend / API      │
                └────────────┬─────────────┘
                             │ REST
                             ▼
        ┌────────────────────────────┐
        │     BillingService (.NET)  │
        │  - Domain / Application    │
        │  - Infrastructure / WebApi │
        └───────────┬────────────────┘
                    │ HTTP (AuditClient)
                    ▼
        ┌────────────────────────────┐
        │     AuditService (Rails)   │
        │  - MVC + DTO + Repositorios│
        └───────────┬────────────────┘
                    │
     ┌──────────────┴───────────────┐
     │                              │
┌────────────┐              ┌────────────┐
│  Oracle DB │              │ MongoDB DB │
│ Facturas & │              │ Eventos de │
│ Clientes   │              │ Auditoría  │
└────────────┘              └────────────┘
```

---



## 🧪 Ejecución de Tests

Para ejecutar las pruebas es necesario encontrarse en un **entorno de desarrollo**.

### ✨ AuditService (Ruby on Rails)
Desde la raíz del proyecto:
```bash
rails test
```

### 🔧 BillingService (.NET 8)
Desde el directorio de la solución:
```bash
dotnet test
```

Ambos conjuntos de pruebas validan la integridad de los componentes principales y las interacciones entre capas.

---


## 🧠 Reflexiones finales

✨ Este proyecto combina **tecnologías modernas** (Ruby on Rails, .NET 8, Docker, Oracle, MongoDB) bajo **principios arquitectónicos sólidos**: bajo acoplamiento, alta cohesión y responsabilidad bien definida.

🧩 Se emplearon patrones **DTO, Repositorio, Validador, Middleware e Inyección de Dependencias**, logrando una estructura limpia, mantenible y extensible.

💬 Al no incluir autenticación, se asume que todo usuario tiene permisos administrativos, lo cual simplifica la prueba sin comprometer la arquitectura.

⚡ En resumen, el sistema cumple todos los requerimientos de la prueba:
- Microservicios independientes.
- Persistencia híbrida (Oracle + MongoDB).
- Principios de Clean Architecture y MVC.
- Se usan UUID seguros, confiables en ambos Microservicios.
- Contenedores Docker con ejecución inmediata.
- API REST funcional con auditoría integrada.

---

## ⚠️ IMPORTANTE

El docker compose crea absolutamente todo lo necesario, incluyendo los servidores de bases de datos. De modo que si ya cuenta con motores instalados en su sistema, debe habilitar y tener libres los siguientes puertos:

- 8080: .NET CORE, Swagger
- 1521: Oracle
- 3000: RAILS, auditoría
- 27017: MongoDB

