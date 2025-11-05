# 🧱 Proyecto DVP – Microservicios Audit y Billing

## 📘 Descripción General

Este proyecto implementa una arquitectura basada en microservicios completamente dockerizada.  
Consta de dos servicios principales:

- **Audit Service (Rails)** → Se conecta a MongoDB.  
- **Billing Service (.NET Core)** → Se conecta a Oracle.

Ambas bases de datos están dockerizadas, y el entorno incluye servicios adicionales para pruebas automatizadas.

El propósito es disponer de un entorno de desarrollo y despliegue **totalmente automatizado y reproducible**, donde cada microservicio y su base de datos puedan levantarse y comunicarse de manera independiente o coordinada.

---

## ⚙️ Requisitos Previos

Antes de ejecutar el proyecto, asegúrate de tener instalado:

- 🐳 **Docker Engine** (versión reciente recomendada)  
- 🧩 **Docker Compose**  
- 💻 Un cliente de base de datos (recomendado **DBeaver**) para inspeccionar las tablas.

---

## ⚠️ Verificación de Puertos Disponibles

Antes de levantar los contenedores, asegúrate de que los siguientes puertos **no estén siendo utilizados** por otros servicios en tu máquina:

| Servicio | Puerto | Descripción |
|-----------|--------|-------------|
| Oracle XE | `1521` | Puerto por defecto del servicio Oracle Database |
| MongoDB   | `27017` | Puerto de conexión de MongoDB |
| Rails (Audit) | `3000` | Puerto de la API del microservicio Rails |
| .NET (Billing) | `8080` | Puerto de la API del microservicio .NET |

Si alguno de ellos está ocupado, libera el puerto o actualiza el archivo `docker-compose.yml` antes de ejecutar el script.

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
Define la implementación de los contratos que deben cumplir la capa de Dominio.

#### 3. 🗄️ **Infrastructure**
Implementa los repositorios (Oracle con EF Core), clientes HTTP (para AuditService), migraciones y los contextos de persistencia.

#### 4. 🌍 **WebApi**
Contiene los controladores, middlewares, pipeline de configuración, inyección de dependencias, validaciones y variables de entorno.  

### ⚙️ Configuración del entorno

- Base de datos Oracle: `gvenzl/oracle-xe`
- Conexión configurada por variables de entorno (en `docker-compose.yml`)
- Swagger habilitado en: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

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

## 🚀 Ejecución del Proyecto

Toda la lógica de inicialización y dependencias se encuentra automatizada en el script **`run_all.sh`**.  
Este script:

1. Construye las imágenes necesarias de Oracle, MongoDB y Rails.  
2. Inicia los contenedores base.  
3. Espera automáticamente hasta que Oracle esté completamente listo. Puede demorar entre 3 o 5 minutos, así que no hay que creer que se queda colgado. 
4. Crea el usuario de aplicación `dvp` con la contraseña `TuContrasena123`.  
5. Asigna todos los permisos requeridos.  
6. Finalmente, levanta el microservicio `.NET` (`billing-service-dvp`).  

De esta forma, **todo el entorno se levanta con un solo comando**.

### ▶️ En Linux o macOS

Abre una terminal en la raíz del proyecto (donde está el `docker-compose.yml`) y ejecuta:

```bash
chmod +x run_all.sh
./run_all.sh
```

El script se encargará del resto.  
Podrás observar los mensajes de progreso y logs en tiempo real directamente desde la terminal.

---

### 🪟 En Windows (Docker Desktop o clientes equivalentes)

Si utilizas **Docker Desktop**, **Rancher Desktop** o cualquier otro cliente que provea una **consola Linux**,  
simplemente abre dicha consola desde la interfaz del cliente y navega a la raíz del proyecto, por ejemplo:

```bash
cd /mnt/c/Proyectos/prueba-dvp
chmod +x run_all.sh
./run_all.sh
```

Docker Desktop y herramientas similares ofrecen un entorno Linux interno, por lo que el script funcionará exactamente igual que en sistemas Linux nativos.  
No se requiere ninguna versión especial ni adaptación para Windows.

---

## 🔗 Acceder a los servicios:
   - Swagger (.NET): [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html) verás los endpoints que expone el ms de facturación. También puedes consumirlos con Postman u otro.
   - Rails Audit API: [http://localhost:3000](http://localhost:3000) Esta URL es solo la base y no mostrará nada en un navegador. Ver Ejemplo de consulta por ID de factura para obtener el endpoint completo. Requiere de Postman u otro.

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

## 🧰 Conexión a la Base de Datos

Una vez el entorno esté en funcionamiento, puedes conectarte a las bases de datos para inspeccionar las tablas creadas por los microservicios.

### 🔸 Oracle (Billing Service)

Puedes usar **DBeaver** o cualquier cliente SQL compatible para conectarte a Oracle con los siguientes datos:

| Parámetro | Valor |
|------------|-------|
| **Usuario** | `dvp` |
| **Contraseña** | `TuContrasena123` |
| **Base de datos (service name)** | `XEPDB1` |
| **Host** | `localhost` |
| **Puerto** | `1521` |

Las tablas principales disponibles serán:  
- `CUSTOMERS`  
- `INVOICES`

### 🔸 MongoDB (Audit Service)

Para inspeccionar los datos de auditoría, conecta con:

| Parámetro | Valor |
|------------|-------|
| **Usuario** | `root` |
| **Contraseña** | `TuContrasena123*` |
| **Host** | `localhost` |
| **Puerto** | `27017` |
| **Base de datos** | `audit_service_prod` |

no se creará ninguna colección hasta que no se cree un log de auditoría desde el ms de facturación o manualmente usando postman.

---

## 🧩 Servicios Incluidos

El archivo `docker-compose.yml` define todos los servicios necesarios:

- **oracle-db-dvp** → Base de datos Oracle XE 21c.  
- **mongodb-dvp** → Base de datos MongoDB.  
- **audit-service-dvp** → Microservicio en Ruby on Rails (producción).  
- **billing-service-dvp** → Microservicio en .NET Core (producción).  
- Servicios equivalentes para pruebas (`*-test`).

Cada servicio se encuentra dentro de la misma red `dvp-network` para permitir comunicación interna.

---

## 🧪 Tests Automatizados

El entorno incluye contenedores dedicados a pruebas unitarias y de integración:  
- `audit-service-test` (Rails)  
- `billing-service-test` (.NET)

Estos pueden levantarse individualmente si se requiere validación aislada:

```bash
docker compose up audit-service-test
docker compose up billing-service-test
```

Los Tests de Rails incluyen tests en el controlador, en la implementación de servicios y en la implementación de repositorios. En el caso de .Net la prueba exige tests sobre la capa de dominio y esos fueron los test realizados. Habría sido interesante que fueran exigidos sobre la capa de aplicación ya que allí se aloja la lógica de negocio.

---

## 🧠 Notas Finales

- El script `run_all.sh` automatiza completamente la creación del usuario Oracle, la espera de readiness y el despliegue coordinado de microservicios.  
- Si en algún momento deseas modificar la contraseña o el usuario, recuerda actualizar tanto el script como las variables de entorno en el `docker-compose.yml`.  
- Al no incluir autenticación, se asume que todo usuario tiene permisos administrativos, lo cual simplifica la prueba sin comprometer la arquitectura.
- Microservicios independientes.
- Persistencia híbrida (Oracle + MongoDB).
- Principios de Clean Architecture y MVC.
- Se usan UUID seguros, aumentando la confiabilidad y seguridad del sistema en ambos Microservicios.
- Contenedores Docker con ejecución inmediata y automátizada.
- API REST funcional con auditoría integrada.
