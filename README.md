# 💰 Sistema de Gestión de Billetera de Criptomonedas

Aplicación web desarrollada como proyecto académico para la **Tecnicatura Universitaria en Programación (UTN)**.

El objetivo del sistema es administrar clientes y sus transacciones de criptomonedas mediante una arquitectura cliente-servidor, integrando una API externa para obtener cotizaciones en tiempo real.

---

# 📌 Descripción

La aplicación permite gestionar clientes, registrar operaciones de compra y venta de criptomonedas y consultar cotizaciones actualizadas utilizando la API de **CriptoYa**.

El proyecto fue desarrollado utilizando una arquitectura por capas, separando responsabilidades entre controladores, servicios, modelos y acceso a datos para mantener un código organizado, escalable y fácil de mantener.

---

# 🚀 Características

- Gestión de clientes.
- Registro de compras y ventas de criptomonedas.
- Consulta de cotizaciones en tiempo real.
- Integración con la API CriptoYa.
- API REST desarrollada con ASP.NET Core.
- Operaciones CRUD.
- Base de datos SQL Server.
- Arquitectura por capas.
- DTOs para intercambio de información.
- Documentación interactiva mediante Swagger.
- Pruebas de endpoints utilizando Postman.
- Frontend desarrollado en Vue.js.

---

# 🛠 Tecnologías utilizadas

## Backend

- C#
- ASP.NET Core
- ASP.NET Core Web API
- Entity Framework Core
- REST API

## Frontend

- Vue 3
- JavaScript
- HTML5
- CSS3
- Axios

## Base de datos

- SQL Server

## Herramientas

- Git
- GitHub
- Swagger
- Postman
- Visual Studio
- Visual Studio Code
- SQL Server Management Studio

---

# 🏗 Arquitectura

El proyecto está dividido en dos aplicaciones independientes.

```
BilleteraCripto
│
├── Backend
│   ├── Controllers
│   ├── Services
│   ├── DTOs
│   ├── Models
│   ├── Data
│   └── Migrations
│
└── Frontend
    ├── Components
    ├── Views
    ├── Router
    ├── Services
    └── Assets
```

El backend expone una API REST que es consumida por el frontend desarrollado en Vue.js.

---

# ⚙ Funcionalidades principales

## Clientes

- Crear clientes.
- Modificar clientes.
- Eliminar clientes.
- Consultar clientes.
- Gestión del saldo disponible.

---

## Transacciones

- Registrar compras.
- Registrar ventas.
- Consultar historial.
- Cálculo automático del monto según la cotización.

---

## Cotizaciones

- Integración con la API de CriptoYa.
- Obtención de precios actualizados.
- Conversión automática de valores.

---

# 📂 API REST

Algunos de los endpoints disponibles:

### Clientes

```
GET    /api/clientes

GET    /api/clientes/{id}

POST   /api/clientes

PUT    /api/clientes/{id}

DELETE /api/clientes/{id}
```

### Transacciones

```
GET    /api/transacciones

GET    /api/transacciones/{id}

POST   /api/transacciones

PUT    /api/transacciones/{id}

DELETE /api/transacciones/{id}
```

---

# 📷 Capturas

> Próximamente se agregarán capturas del sistema y del frontend.

---

# ▶ Cómo ejecutar el proyecto

## Clonar el repositorio

```bash
git clone https://github.com/GastonRossoEsp/BilleteraCripto.git
```

---

## Backend

```bash
cd backend

dotnet restore

dotnet ef database update

dotnet run
```

---

## Frontend

```bash
cd frontend

npm install

npm run dev
```

---

# 📚 Lo que aprendí

Durante el desarrollo de este proyecto adquirí experiencia en:

- Desarrollo de APIs REST.
- Arquitectura por capas.
- Programación orientada a objetos.
- Diseño de bases de datos relacionales.
- Integración entre frontend y backend.
- Consumo de APIs externas.
- Gestión de dependencias.
- Documentación de APIs con Swagger.
- Control de versiones utilizando Git y GitHub.

---

# 🚧 Estado del proyecto

🔨 En desarrollo.

Actualmente continúo incorporando nuevas funcionalidades y realizando mejoras tanto en el backend como en el frontend.

---

# 🎯 Próximas mejoras

- Sistema de autenticación.
- Validaciones avanzadas.
- Dashboard con estadísticas.
- Historial detallado de transacciones.
- Mejoras en la interfaz de usuario.
- Despliegue en la nube.
- Dockerización del proyecto.
- Pruebas automatizadas.

---

# 👨‍💻 Autor

**Gastón Rafael Rosso Tonini**

📧 gastonrossotonini@gmail.com

💼 LinkedIn

https://www.linkedin.com/in/gaston-rosso-tonini-251085287

💻 GitHub

https://github.com/GastonRossoEsp
