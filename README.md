# ♟️ ChessApp – API REST

**ChessApp** es un proyecto personal desarrollado desde cero utilizando **ASP.NET Core**. Se trata de una API REST robusta diseñada bajo el patrón arquitectónico **MVC (Modelo-Vista-Controlador)**, el cual aplica una separación clara de responsabilidades entre controladores, servicios y lógica de negocio. El sistema cuenta con gestión completa de usuarios (registro e inicio de sesión) y utiliza **Supabase (PostgreSQL)** como motor principal para la persistencia de datos, garantizando una estructura altamente escalable, segura y fácil de mantener.

---

## 🚀 Características Principales

* **Arquitectura Limpia (MVC decoupled):** Separación estricta entre la capa de presentación (Controladores), la capa de orquestación (Servicios) y la capa de datos/negocio.
* **Autenticación y Autorización:** Sistema seguro para el registro de nuevos usuarios y login.
* **Persistencia en la Nube:** Integración nativa con **Supabase** aprovechando la potencia y fiabilidad de **PostgreSQL**.
* **Código Escalable:** Estructura modular diseñada para facilitar la incorporación de nuevas reglas de negocio (como el backend para partidas de ajedrez, movimientos y emparejamientos).

---

## 🛠️ Stack Tecnológico

* **Framework:** .NET / ASP.NET Core
* **Base de Datos:** PostgreSQL (Alojado en Supabase)
* **Arquitectura:** REST API con patrón MVC y Capa de Servicios independientes.

---

## 📂 Estructura del Proyecto

El código está organizado siguiendo las mejores prácticas de desarrollo de APIs para desacoplar responsabilidades:

* **`Controllers/`**: Manejan las peticiones HTTP entrantes, validan los datos de entrada mínimos y devuelven las respuestas correspondientes (JSON).
* **`Services/`**: Contienen la lógica de negocio central de la aplicación, sirviendo de puente entre los controladores y la base de datos.
* **`Models/`**: Definen las entidades del sistema (Usuarios, Partidas, etc.) y los DTOs (Data Transfer Objects) para la transferencia segura de información.
* **`Data/` / `Repositories/`**: Gestionan las conexiones, consultas y mutaciones directamente en Supabase/PostgreSQL.
