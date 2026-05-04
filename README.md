# SimpleStockMarket

SimpleStockMarket is a backend service that simulates a small stock exchange with bank inventory, user wallets, buy/sell operations and audit logging. The project is implemented in .NET 10 with Entity Framework Core and PostgreSQL, and it is packaged to run fully in Docker.

## Single-command Startup

The whole system is started with one command in bash:

        ./run.sh {PORT} {INSTANCES}

`{PORT}` is the host port used for the application.

`{INSTANCES}` is the number of API instances running behind the reverse proxy.

If arguments are omitted, the script falls back to default values.

## How to Run

1.  Make sure Docker Desktop is running.

2.  Start the system with:

        ./run.sh 8000 2

3.  Open the application at:
    `http://localhost:8000`

    The PORT argument can be changed to any free port on the host machine. The second argument controls how many API replicas are started.

## Technical Overview

The codebase follows a layered backend architecture:

- HTTP controllers expose the public API.
- Services contain business rules and transaction flow.
- Repositories encapsulate persistence and atomic database updates.
- Entity Framework Core with PostgreSQL is used for data access.
- DTOs and mappers isolate the API contract from the entity model.

The main request flow is:

```text
HTTP -> Controller -> Service -> Repository -> PostgreSQL -> Repository -> Service -> Controller -> HTTP
```

This separation was chosen to keep the business logic testable and to allow repository methods to be covered by integration tests while service methods are covered by unit tests.

## Cross-platform and Architecture Support

The solution is designed to run on Windows, Linux, and macOS, and it uses official Docker images that support both arm64 and x64 architectures. No machine-specific dependencies are required beyond Docker and a Bash-compatible shell for the launcher script.

## Availability and Scalability

The application is deployed as multiple API containers behind Nginx. The load-balancer service forwards traffic to the replicated rest-api instances, so killing one instance does not stop the whole product. This gives basic high availability at the container level and allows horizontal scaling through the --scale parameter in the startup script.

## Environment Assumptions

The solution does not assume any local runtime beyond what is required to run Docker. All application dependencies are packaged inside the container images, and PostgreSQL is provisioned as part of the compose stack.

## Implementation Details

### Backend

- ASP.NET Core Web API targeting .NET 10.
- PostgreSQL database accessed through Entity Framework Core.
- Atomic stock updates are implemented with EF Core bulk update operations.
- Audit logs are stored separately from stock and wallet state.
- Business operations such as buy and sell are handled in a dedicated service layer.

### Infrastructure

- `Infrastructure/Dockerfile` uses a multi-stage build.
- Tests are executed during the image build stage.
- The final runtime image only contains published application output.
- `Infrastructure/docker-compose.yml` starts PostgreSQL, the API containers, and the Nginx reverse proxy.
- `Infrastructure/nginx.conf` exposes the application on the selected host port and forwards requests to the API instances.

### Testing strategy

- Repository methods are covered by integration tests against PostgreSQL.
- Service methods are covered by unit tests with mocks.
- The test suite was developed incrementally with TDD-style coverage.

## Project Structure

- `SimpleStockMarket/` - application source code.
- `SimpleStockMarket/Controllers/` - HTTP endpoints.
- `SimpleStockMarket/Services/` - business logic.
- `SimpleStockMarket/Repositories/` - persistence layer.
- `SimpleStockMarket/DataBase/` - DbContext and database configuration.
- `SimpleStockMarket/Entities/` - EF Core entities.
- `SimpleStockMarket/DTOs/` - request/response models.
- `SimpleStockMarket/Mappers/` - conversion between entities and DTOs.
- `Infrastructure/` - Docker, compose, and reverse proxy configuration.
- `Tests/` - unit and integration tests.

## Notes

The .env and appsettings.json files have been intentionally added to the GitHub repository to enable running the project with a single terminal command. Usually, these files should never be committed, but the target of this project wasn't to expose it to production and real endpoints.

There was also an option to hardcode values that are in .env directly in the docker-compose.yml, but I wanted to show that I can integrate things properly.

## Author

Mateusz Sadowski
