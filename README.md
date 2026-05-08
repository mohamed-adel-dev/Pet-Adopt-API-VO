# 🐾 Pet Adoption System — .NET 10 Web API

Layered Architecture | ASP.NET Core 10 | EF Core | SQL Server | SignalR | Docker

---

## 📌 Overview

Pet Adoption System is a RESTful Web API built with ASP.NET Core 10 that helps shelters publish pets for adoption while allowing adopters to browse pets, send adoption requests, save favorites, and leave feedback.

The project follows a layered architecture with clean separation between:

* API Layer
* Business Logic Layer
* Data Access Layer

It also includes:

* ASP.NET Identity authentication & roles
* Real-time notifications using SignalR
* Docker support
* Scalar API documentation
* Entity Framework Core with SQL Server

---

# 🏗️ Architecture

```text
PetAdopt/
│
├── PetAdopt.API/        → Presentation Layer
│   ├── Controllers/
│   └── Hubs/
│
├── PetAdopt.BLL/        → Business Logic Layer
│   ├── DTOs/
│   ├── Services/
│   └── Interfaces/
│
├── PetAdopt.DAL/        → Data Access Layer
│   ├── Entities/
│   ├── Repositories/
│   └── Data/
│
├── Dockerfile
├── README.md
└── PetAdopt.sln
```

---

# 🚀 Features

## 🔐 Authentication & Authorization

* Register & Login
* ASP.NET Identity
* Role-based authorization

### Roles

* Admin
* Shelter
* Adopter

---

## 👨‍💼 Admin Features

* Approve users
* Reject users
* Approve pet posts
* Reject pet posts
* View pending users
* View pending pets

---

## 🐾 Pet Features

* Create pet posts
* Update pet posts
* Delete pet posts
* Browse approved pets
* Search pets
* Get pet details

---

## ❤️ Favorites

* Add pet to favorites
* Remove pet from favorites
* Get adopter favorites

---

## 📩 Adoption Requests

* Send adoption request
* Accept adoption request
* Reject adoption request
* View shelter requests

---

## ⭐ Feedback

* Submit feedback
* Get pet feedbacks

---

## 🔔 SignalR Notifications

Real-time notifications for:

* New adoption requests
* Accepted requests
* Rejected requests

---

# 🛠️ Technologies

* ASP.NET Core 10
* Entity Framework Core
* SQL Server
* ASP.NET Identity
* SignalR
* Docker
* Scalar OpenAPI

---

# ⚙️ Running Locally

## 1️⃣ Clone the repository

```bash
git clone <YOUR_REPOSITORY_URL>
cd PetAdopt
```

---

## 2️⃣ Restore packages

```bash
dotnet restore
```

---

## 3️⃣ Apply migrations

```bash
dotnet ef database update
```

---

## 4️⃣ Run the project

```bash
dotnet run --project PetAdopt.API
```

---

# 📄 API Documentation

Scalar UI:

```text
https://localhost:7003/scalar/v1
```

---

# 🐳 Docker

## Build Docker image

```bash
docker build -t petadopt-api .
```

---

## Run Docker container

```bash
docker run -d -p 8080:8080 --name petadopt-container petadopt-api
```

---

## Open API

```text
http://localhost:8080/scalar/v1
```

---

# 🔑 Default Admin Account

```text
Email: admin@admin.com
Password: AdminTest@1234
```

---

# 🔔 SignalR Hub

```text
/notificationHub
```

---

# 📦 API Endpoints

## Admin

| Method | Endpoint                           | Description       |
| ------ | -----------------------------------| ----------------- |
| GET    | `/api/Admin/pending-users`         | Get pending users |
| GET    | `/api/Admin/pending-pets`          | Get pending pets  |
| PUT    | `/api/Admin/approve-user/{userId}` | Approve user      |
| PUT    | `/api/Admin/approve-pet/{petId}`   | Approve pet       |
| PUT    | `/api/Admin/reject-pet/{petId}`    | Reject pet        |

---

## Pets

| Method | Endpoint                |
| ------ | ----------------------- |
| GET    | `/api/Pet/all`          |
| GET    | `/api/Pet/details/{id}` |
| GET    | `/api/Pet/search`       |
| POST   | `/api/Pet/create`       |
| PUT    | `/api/Pet/update/{id}`  |
| DELETE | `/api/Pet/delete/{id}`  |
| GET    | `/api/Pet/my-pets`      |

---

## Adoption Requests

| Method | Endpoint									  |	
| ------ | -------------------------------------------|
| POST   | `/api/AdoptionRequest/send`				  |		
| PUT    | `/api/AdoptionRequest/accept/{requestId}`  |
| PUT    | `/api/AdoptionRequest/reject/{requestId}`  |
| GET    | `/api/AdoptionRequest/owner-requests`      |

---

## Favorites

| Method | Endpoint                                    |
| ------ | --------------------------------------------|
| POST    | `/api/Favorite/add`						   |
| GET     | `/api/Favorite/my-favorites/{adopterId}`   |
| DELETE  | `/api/Favorite/remove/{petId}​`             |

---

## Feedback

| Method | Endpoint                    |
| ------ | --------------------------- |
| POST   | `/api/Feedback/create`      |
| GET    | `/api/Feedback/pet/{petId}` |

---

# 📌 Notes

* Pet posts require admin approval before appearing publicly.
* Newly registered users start with `Pending` status.
* SignalR notifications work in real time.
* Dockerized for easy deployment.

---

# 👨‍💻 Author

Mohammed Adel
