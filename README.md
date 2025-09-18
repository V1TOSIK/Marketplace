# 🛒 Marketplace API

[![.NET 8](https://img.shields.io/badge/.NET-8-purple)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/DB-PostgreSQL-blue)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Cache-Redis-red)](https://redis.io/)
[![Docker](https://img.shields.io/badge/Docker-Ready-informational?logo=docker)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## 📌 Опис  
**Marketplace API** — це **backend частина маркетплейсу**, реалізована на **.NET 8** з використанням **CQRS, MediatR та Domain-Driven Design (DDD)**.  
API включає **автентифікацію, управління користувачами, продуктами, медіа та інтеграції з зовнішніми сервісами**.  

---

## 🚀 Основний функціонал  

### 🔐 Auth Module  
- Реєстрація користувачів (**Email / Phone / Google OAuth**)  
- Авторизація за схемою **Access Token + Refresh Token**  
- Керування акаунтом (видалення, відновлення, блокування)  
- Ролі користувачів (**User, Admin, Guest**)  

**Entities**:  
- `AuthUser`  
- `RefreshToken`  

---

### 👤 User Module  
- Профіль користувача (ім’я, дані, контакти)  
- Керування телефонними номерами  
- Синхронізація з AuthModule  

**Entities**:  
- `User`  
- `UserPhoneNumber`  

---

### 📦 Product Module  
- Створення та управління товарами  
- Категорії, характеристики, фільтрація  
- Підтримка пагінації та пошуку  
- Готовність до винесення в мікросервіс  

**Entities**:  
- `Product`  
- `Category`  
- `CharacteristicGroup`  
- `CharacteristicTemplate`  
- `CharacteristicValue`  

---

### 🖼️ Media Module  
- Завантаження та зберігання медіа  
- Інтеграція з **MinIO (S3 сумісне сховище)**  

**Entities**:  
- `Media`  

---

## ⚙️ Використані технології
- **.NET 9 / ASP.NET Core**  
- **CQRS + MediatR**  
- **DDD (Domain-Driven Design)**  
- **Entity Framework Core (PostgreSQL)**  
- **Redis** — кешування  
- **MinIO** — зберігання файлів (S3 Storage)  
- **MailKit** — відправка Email  
- **Twilio** — SMS повідомлення  
- **Google OAuth** — автентифікація  
- **Docker** — контейнеризація  

---

## 🏗️ Архітектура

### 🔹 Високорівнева діаграма  

```mermaid
flowchart TD
    Client[Фронтенд / Mobile App] --> API[Marketplace.API]

    subgraph API[Marketplace API (.NET 8)]
        A[Auth Module] -->|JWT / OAuth| DB[(PostgreSQL)]
        U[User Module] -->|Users, Phones| DB
        P[Product Module] -->|Products, Categories| DB
        M[Media Module] -->|Files Metadata| DB
    end

    API --> Cache[(Redis)]
    API --> Storage[(MinIO S3)]
    API --> Mail[MailKit SMTP]
    API --> SMS[Twilio API]

Marketplace/
├── Marketplace.Api/           # Вхідна точка (Controllers, DI, Middleware)
│
├── AuthModule/                # Авторизація та автентифікація
│   ├── Application/           # CQRS (Commands, Queries, Handlers)
│   ├── Domain/    
│   ├── Composition/            # Entities, ValueObjects, Exceptions
│   ├── Persistence/           # Repositories, DbContext
│   └── Infrastructure/                   # Controllers, DTOs
│
├── UserModule/                # Профіль користувача
│   ├── Application/
│   ├── Domain/
│   ├── Composition/
│   ├── Persistence/
│   └── Infrastructure/
│
├── ProductModule/             # Продукти, категорії, характеристики
│   ├── Application/
│   ├── Domain/
│   ├── Composition/
│   └── Persistence/
│
│
├── MediaModule/               # Завантаження та зберігання медіа
│   ├── Application/
│   ├── Domain/
│   ├── Composition/
│   ├── Persistence/
│   └── Infrastructure/
│
├── SharedKernel/              # Базові абстракції (AggregateRoot, ValueObjects)
│
└── docker-compose.yml