# 🧺 Survey Basket System API

**Survey Basket System** is a powerful, flexible, and secure survey and polling API built with **.NET 8**, **Entity Framework Core**, and **JWT Authentication**.  
This project was a deep dive into modern backend development practices and represents a significant leap in both my knowledge and implementation of scalable, maintainable, and secure web APIs. 🚀

---

## 🚀 Key Features

### 🔐 Authentication & Authorization
- **JWT-Based Authentication** for secure access control
- **User and Role Management** to handle permissions effectively

### 📊 Polls and Surveys
- Create, manage, and participate in interactive polls
- Designed for easy data collection and user engagement

### 📝 Audit Logging
- Tracks changes and actions across the system
- Promotes transparency and accountability

### 🚨 Exception & Error Handling
- **Centralized Exception Handling** for consistent error responses
- **Result Pattern** for structured, predictable error feedback

### 🔄 Efficient Data Handling
- **Automapper / Mapster** for clean object-to-object mapping
- **Fluent Validation** to enforce rules and ensure data quality

### 🔐 Account Management
- Change/reset password functionality
- Email-based account confirmation and password recovery

### 🚦 Rate Limiting
- Protects API from abuse
- Ensures fair resource access for all users

### 🛠 Background Jobs
- **Hangfire Integration** for email confirmations, resets, etc.

### 🔍 Health Checks
- Monitor system uptime and service health

### 🗃️ Distributed Caching
- Speeds up data access for frequently used endpoints

### 📧 Email Features
- Send confirmation emails and manage password processes seamlessly

### 📈 API Versioning
- Multiple version support to maintain backward compatibility

---

## 🧰 Tech Stack

- **Backend:** .NET 8 Web API
- **Authentication:** JWT (JSON Web Tokens)
- **ORM:** Entity Framework Core
- **Validation:** FluentValidation
- **Background Jobs:** Hangfire
- **Caching:** Distributed Caching (e.g., Redis)
- **Email:** SMTP/Email Sender Service
- **Mapping:** Automapper or Mapster
- **Versioning:** ASP.NET Core API Versioning
- **Tools:** Visual Studio, Postman, Swagger (OpenAPI)

---

## 📦 Getting Started

1. **Clone the repo**

```bash
git clone https://github.com/ramez00/SurveyBasket.git
cd survey-basket-system
