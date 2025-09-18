# 🛒 Marketplace API

[![.NET 9](https://img.shields.io/badge/.NET-9-purple)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/DB-PostgreSQL-blue)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Cache-Redis-red)](https://redis.io/)
[![Docker](https://img.shields.io/badge/Docker-Ready-informational?logo=docker)](https://www.docker.com/)

---

## 📌 Опис  
**Marketplace API** — це **backend частина маркетплейсу**, реалізована на **.NET 9** з використанням **CQRS, MediatR та Domain-Driven Design (DDD)**.  
API включає **автентифікацію, управління користувачами, продуктами, медіа та інтеграції з зовнішніми сервісами**.  

---

## 🚀 Основний функціонал  

### 🔐 Auth Module
- Реєстрація користувачів (**Email / Phone / Google OAuth**)  
- Авторизація за схемою **Access Token + Refresh Token**  
- Керування акаунтом (видалення, відновлення, блокування)  
- Ролі користувачів (**User, Admin, Guest**)  

**Entities:** `AuthUser`, `RefreshToken`  

---

### 👤 User Module
- Профіль користувача (ім’я, дані, контакти)  
- Керування телефонними номерами  
- Синхронізація з AuthModule  

**Entities:** `User`, `UserPhoneNumber`  

---

### 📦 Product Module
- Створення та управління товарами  
- Категорії, характеристики, фільтрація  
- Підтримка пагінації та пошуку  
- Готовність до винесення в мікросервіс  

**Entities:** `Product`, `Category`, `CharacteristicGroup`, `CharacteristicTemplate`, `CharacteristicValue`  

---

### 🖼️ Media Module
- Завантаження та зберігання медіа  
- Інтеграція з **MinIO (S3 сумісне сховище)**  

**Entities:** `Media`  

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

```mermaid
flowchart TD
    Client[Frontend or Mobile App] --> API[Marketplace API .NET 9]

    subgraph API[Marketplace API]
        A[Auth Module] -->|JWT or OAuth| DB[PostgreSQL]
        U[User Module] -->|Users and Phones| DB
        P[Product Module] -->|Products and Categories| DB
        M[Media Module] -->|Files Metadata| DB
    end

    API --> Cache[Redis]
    API --> Storage[MinIO S3]
    API --> Mail[MailKit SMTP]
    API --> SMS[Twilio API]
```


---


## 🏗️ Структура проекту

Marketplace/           
├── Marketplace.Api/           
├── AuthModule/                
│   ├── Application/           
│   ├── Domain/    
│   ├── Composition/            
│   ├── Persistence/           
│   └── Infrastructure/        
├── UserModule/               
│   ├── Application/           
│   ├── Domain/    
│   ├── Composition/            
│   ├── Persistence/           
│   └── Infrastructure/        
├── ProductModule/             
│   ├── Application/           
│   ├── Domain/    
│   ├── Composition/            
│   └── Persistence/           
├── MediaModule/                 
│   ├── Application/           
│   ├── Domain/    
│   ├── Composition/            
│   ├── Persistence/           
│   └── Infrastructure/        
├── SharedKernel/              
└── docker-compose.yml

---

## 🏁 Getting Started

### 🔹 Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Docker
- PostgreSQL (можна через Docker)
- Redis (можна через Docker)

### 🔹 Запуск проекту

Зібрати і запустити Docker контейнери для проекту(PostgreSQL, Redis, MinIO, backend, frontend):
```bash
docker-compose up --build -d
```
backend працює на порту 8000