# Backend-Services

How to setup for local development?

1. Clone the repo
2. run 'make sqlup' in the root folder. This will create a docker container with the SQL Server instance
3. run the BackendService application

(run 'make sqldown' to stop the sql server)

For Frontend team: run 'make up' to also start an instance of the backend service application ready to use by the mobile app.

(run 'make down' to stop the backend service application instance)

Alternatively, you can use the same make sqlup command and run the backend service application using Visual Studio or by executing 'dotnet run' in the CLI

# Create database migrations

Requirements:

1. Install dotnet-ef tools: "dotnet tool install --global dotnet-ef"