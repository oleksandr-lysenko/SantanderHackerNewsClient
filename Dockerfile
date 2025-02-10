# Use the official .NET 6 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the project files and restore any dependencies
COPY src/*.csproj ./src/
RUN dotnet restore src/HackerNewsClient.csproj

# Copy the remaining files and build the application
COPY src/. ./src/
WORKDIR /app/src
RUN dotnet publish -c Release -o out

# Use the official .NET 6 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/src/out .

# Expose the port the application runs on
EXPOSE 80

# Define the entry point for the container
ENTRYPOINT ["dotnet", "HackerNewsClient.dll"]