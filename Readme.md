# RankMonkey

RankMonkey is a web application that allows users to compare their income and net worth with others. It provides a simple and private way to see where you stand financially compared to your peers.

## Project Structure

The solution consists of several projects:

- `RankMonkey.Server`: ASP.NET Core Web API backend
- `RankMonkey.Client`: Blazor WebAssembly frontend
- `RankMonkey.Shared`: Shared models and constants
- `RankMonkey.Host`: Aspire host project for orchestrating the application
- `RankMonkey.ServiceDefaults`: Common service configurations

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or later (optional)

### Running the Application

1. Clone the repository
2. Navigate to the solution directory
3. Add the Google Client ID, Client Secret, and JWT Secret to the user secrets:
   ```
   dotnet user-secrets set "Authentication:Google:ClientId" "your_google_client_id"
   dotnet user-secrets set "Authentication:Google:ClientSecret" "your_google_client_secret"
   dotnet user-secrets set "Jwt:SecretKey" "your_jwt_secret_key"
   ```
4. Run the following command:
   ```
   dotnet run --project RankMonkey.Host
   ```
5. Open a web browser and navigate to `https://localhost:6001` to access the application.

Note: The first time you run the application, it may take a few moments to compile and start up all the services.