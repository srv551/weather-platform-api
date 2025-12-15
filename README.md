# Weather Platform API

A modern **.NET 8 Web API** that provides weather data, forecasts, air quality insights,
travel comfort scoring, and rich observability using OpenTelemetry.

The project follows **clean architecture principles**, supports **API versioning**, **rate limiting**,
and demonstrates **production-ready configuration and security practices**.

---

## 🚀 Features

- 🌦 Current weather by city or coordinates  
- 📅 Multi-day weather forecasts  
- 🌍 Time zone lookup  
- 🌅 Astronomy data (sunrise, sunset, moon phases)  
- 📍 IP-based location lookup  
- 🔎 Location search / autocomplete  
- 🧳 Travel & comfort score (0–100)  
- 💡 Weather advice (umbrella, heat, UV, air quality)  
- 🧠 In-memory caching for performance  
- 🧱 API versioning (`/api/v1`)  
- 🚦 Rate limiting  
- 📊 OpenTelemetry (metrics, traces, logs)  
- 📘 OpenAPI documentation (Swagger / Scalar UI)  

---

## 🛠 Tech Stack

- **.NET 8**
- **ASP.NET Core Web API**
- **WeatherAPI.com** (external provider)
- **OpenTelemetry**
  - Tracing
  - Metrics (Prometheus)
  - Logs
- **ASP.NET API Versioning**
- **MemoryCache**
- **Rate Limiting**
- **GitHub Actions (CI)**

---

## 🧱 Architecture

**WeatherApi.Api**
- Controllers
- Middleware
- Swagger
- Program.cs

**WeatherApi.Application**
- DTOs
- Interfaces

**WeatherApi.Infrastructure**
- External  
  - WeatherAPI models
- Services
- Config

---

This separation ensures:
- Clear responsibility boundaries.  
- Testability and maintainability.  
- Easy extension for future features.  

---

## ▶️ Running Locally

### 1️⃣ Prerequisites
- .NET 8 SDK  
- WeatherAPI.com API key  

---

### 2️⃣ Local configuration (NOT committed)

Create a file named **`appsettings.Local.json`** inside **`WeatherApi.Api`**:

```json
{
  "WeatherApi": {
    "BaseUrl": "https://api.weatherapi.com/v1/",
    "ApiKey": "YOUR_LOCAL_WEATHERAPI_KEY"
  }
}
```
This file is ignored by Git and should never be committed.

###3️⃣ Run the API

From the solution root:
dotnet restore
dotnet build
dotnet run --project WeatherApi.Api

###4️⃣ Open API documentation

Scalar UI:
- https://localhost:<port>/docs

OpenAPI JSON:
- /openapi/v1.json

🔐 Configuration & Secrets
- Secrets are never committed to source control.

Supported approaches:
- appsettings.Local.json (ignored by Git)
- dotnet user-secrets (local development)
- Environment variables:
  - WeatherApi__ApiKey

Configuration precedence
- appsettings.json
- appsettings.{Environment}.json
- User Secrets (Development only)
- Environment variables

📈 Observability
The API is instrumented with OpenTelemetry:
- Traces: HTTP requests and outbound calls
- Metrics: runtime, HTTP, and custom metrics
- Logs: structured logs with trace correlation

Metrics endpoint:
- /metrics
(Prometheus-compatible)

🚦 Rate Limiting
Global rate limiting is enabled to protect the API from abuse and ensure fair usage.
Limits are configurable and applied consistently across endpoints.

🧪 Continuous Integration
The repository includes a GitHub Actions CI pipeline that:
- Restores dependencies
- Builds the solution
- Runs tests (when present)
CI runs automatically on:
- Pushes to main
- Pull requests targeting main

📌 Author Notes
This project demonstrates:
- Real-world API design in .NET 8
- Clean architecture and layering
- Secure configuration management
- Observability-first mindset
- Production-ready practices

Feel free to fork, explore, or extend this project.

---

## ✅ What to do next

1. Paste this into `README.md` at repo root  
2. Commit:
   ```bash
   git add README.md
   git commit -m "Add project README"
   git push
3. Check GitHub → repo homepage → README renders cleanly
