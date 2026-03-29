Perfect — now let’s upgrade your README to **🔥 recruiter-level / SaaS startup quality**
This is the kind that makes hiring managers stop and look.

👉 Copy everything below into your `README.md`

---

# 🚀 Threadly

### A Modern Social SaaS Platform for Media-Driven Conversations

![.NET](https://img.shields.io/badge/.NET-10-blue)
![Blazor](https://img.shields.io/badge/Blazor-Server-purple)
![EF Core](https://img.shields.io/badge/EF%20Core-SQLite-green)
![Status](https://img.shields.io/badge/status-active-success)

---

## 💡 What is Threadly?

**Threadly** is a full-stack SaaS-style social platform where users can create, share, and interact with content using **text, images, and videos**.

It is designed to simulate a **real-world scalable product**, focusing on:

* Clean architecture
* API-driven development
* Modern UI/UX
* Media handling
* Database integration

> Built as a portfolio-ready system demonstrating real software engineering practices.

---

## ✨ Key Features

* 📝 Create posts with title + body
* 🖼 Upload images and videos
* ❤️ Like / Unlike posts
* 💬 Comment system
* 🔍 Search & filter posts
* 📦 Preloaded database with demo data
* ⚡ Blazor interactive UI
* 📁 Media storage with static file serving

---

## 🎬 Preview

> Replace these with your screenshots or GIFs

```md
## 🎬 Preview

### 🏠 Home Feed
<img width="1341" height="927" alt="Screenshot 2026-03-29 132833" src="https://github.com/user-attachments/assets/3cdf2321-5920-45ed-8fee-67c9bf0f76aa" />


### 📝 Create Post with Media
![Create](./preview/create.png)

### 📄 Post Details View
![Details](./preview/details.png)
```

👉 Create this folder:

```
/preview
```

---

## 🧠 Architecture Overview

```text
Blazor UI (Client)
        ↓ HTTP
ASP.NET Web API (Controllers)
        ↓
Repositories (EFC)
        ↓
SQLite Database + Media Files
```

### 🧩 Design Principles

* Separation of Concerns
* Repository Pattern
* DTO-based communication
* Scalable API structure

---

## 🛠 Tech Stack

### Frontend

* Blazor Server
* Razor Components
* Bootstrap

### Backend

* ASP.NET Core Web API
* Entity Framework Core

### Database

* SQLite (with migrations)

### Other

* REST APIs
* Dependency Injection
* Static file hosting for media

---

## ⚙️ Getting Started

### 1. Clone project

```bash
git clone https://github.com/Waqarahmedkhan96/Threadly.git
cd Threadly
```

---

### 2. Run Backend

```bash
dotnet run --project Server/WebApi/WebApi.csproj
```

API runs at:

```
https://localhost:5235
```

---

### 3. Run Frontend

```bash
dotnet run --project Client/BlazorApp/BlazorApp.csproj
```

---

## 🗄 Data & Media

This project includes:

* ✅ Preconfigured SQLite database (`app.db`)
* ✅ Sample users, posts, likes, comments
* ✅ Uploaded media files

👉 So anyone cloning the repo gets **instant working demo**

---

## 📁 Project Structure

```text
Threadly/
│
├── Client/BlazorApp        → Frontend (Blazor UI)
├── Server/WebApi           → API + Uploads + DB
├── Server/EfcRepositories  → EF Core logic
├── Server/Entities         → Domain models
├── Shared/ApiContracts     → DTOs
```

---

## 🔌 API Highlights

```http
POST   /posts
POST   /posts/upload
GET    /posts
GET    /posts/{id}
PUT    /posts/{id}
DELETE /posts/{id}
POST   /posts/{id}/likes
```

---

## 📊 Why This Project Matters

This project demonstrates:

* Full-stack development (Frontend + Backend)
* Real API design
* Database modeling with EF Core
* File/media handling in production-like setup
* Clean architecture used in industry

👉 This is **not just a CRUD app** — it's a **mini SaaS system**

---

## 🚧 Future Improvements

* 🔐 JWT Authentication
* 👤 User profiles
* ☁️ Cloud deployment (Azure / AWS)
* 🐳 Docker + Kubernetes support
* 🔔 Notifications
* 📊 Admin dashboard

---

## 🤝 Contributing

Feel free to fork and improve the project.

---

## 👨‍💻 Author

**Waqar Ahmed Khan**
Software Engineering Student @ VIA University College

* GitHub: [https://github.com/Waqarahmedkhan96](https://github.com/Waqarahmedkhan96)
