<div align="center">
    <img src=".github/assets/mirage.png" width="420" alt="Terestrium logo">

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-13.0-239120?style=flat-square&logo=c-sharp)

Terestrium is a simple 2D online RPG (ORPG) engine written in C#. 

Inspired by the classic VB6 Mirage 3.0.3 engine.

</div>

## Features

- Account management
- Character creation
- Basic combat system
- NPCs
- Items (Inventory + Equipment)

## Requirements

- .NET SDK 9.0 or later
- Docker and Docker Compose (for required services)

## Quick Start

### 1) Start required services
```bash
# From the repository root
docker-compose up -d
```

### 2) Run the Server
```bash
cd src/Mirage.Server
dotnet run
```
The server will initialize and be ready to accept client connections. Ensure the Docker services are running first.

### 3) Run the Client (optional)
You can run the client from the Terestrium client project:
```bash
cd src/Terestrium.Client
dotnet run
```
Alternatively, from the repo root:
```bash
dotnet run --project src/Terestrium.Client/Terestrium.Client.csproj
```

## Stopping

- Stop the server: press Ctrl+C in the terminal where it's running
- Stop Docker services when finished:
```bash
docker-compose down
```

## Contributing

Contributions are welcome! If you encounter issues or have suggestions or feature requests, please open an issue or pull request.

## License

This project is licensed under the terms of the LICENSE file included in this repository.
