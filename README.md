## TemplateApi (.NET 10) ✨
Starter Web API template using .NET 10 with example controllers, services, mapping and middleware. It provides a foundation for building Web APIs with JWT authentication, global exception handling, and Swagger.<br>
Status: template ready for customization. <br>
It´s a copy of https://github.com/agustinafassina/TemplateApi.Net8

## Requirements ✅
- 🧰 .NET 10 SDK
- 🐳 (Optional) Docker

## Main contents 📦
- `Program.cs`: application entry point and service configuration.
- `Controllers/`: REST controllers (e.g., `ItemController`).
- `Services/`: business logic and interfaces.
- `Middleware/`: global handlers (e.g., `GlobalExceptionHandlerMiddleware`).
- `appsettings.json` / `appsettings.Development.json`: environment-specific configuration.

## Run locally ▶️
1. Restore and build:
```
dotnet build
```

2. Run the API:
```
dotnet run --project TemplateApi.csproj
```

By default, when running in Development, Swagger should be available at `http://localhost:{port}/swagger`.

## Docker 🐳
Build the image and run the container:
```
docker build -f Dockerfile -t templateapi:latest .
docker run -d -p 8787:80 -e "ASPNETCORE_ENVIRONMENT=Development" --name templateapi templateapi:latest
# Swagger: http://localhost:8787/swagger/index.html
```

## Authentication (JWT / Auth0 example) 🔐
The template includes support for JWT authentication. Configure the values in `appsettings.json` or via environment variables. Example JSON configuration:
```
"Auth0App1": {
  "Issuer": "https://your-domain.auth0.com/",
  "Audience": "Your-Audience"
},
"Auth0App2": {
  "Issuer": "https://another-issuer/",
  "Audience": "Another-Audience"
}
```

And in controllers you can use:
```
[Authorize(AuthenticationSchemes = "Auth0App1")]
[Authorize(AuthenticationSchemes = "Auth0App2")]
```

## Configuration ⚙️
- Use `appsettings.json` and `appsettings.Development.json` for environment-specific values.
- Environment variables prefixed with `ASPNETCORE_` affect host behavior.

## Example: Version endpoint ⚡

This template exposes a simple version endpoint in `ItemController`.

- Request:

  ```
  GET /api/v1/item/version
  ```

- curl example:

  ```bash
  curl -i http://localhost:5000/api/v1/item/version
  ```

- Response 200 (example):

  ```json
  "v.1.0.0"
  ```

## Contributing 🤝
1. Fork the repo
2. Create a branch (`feature/name`)
3. Open a Pull Request describing your changes

## License 📜
This repository is a template created by Agustina Fassina.