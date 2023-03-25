# Backend-Services

How to setup for local development?

1. Clone the repo
2. run 'make sqlup' in the root folder. This will create a docker container with the SQL Server instance
3. run the BackendService application

For Frontend team: run 'make up' to also start an instance of the backend service application ready to use by the mobile app

# Create database migrations

Requirements:

1. Install dotnet-ef tools: "dotnet tool install --global dotnet-ef"