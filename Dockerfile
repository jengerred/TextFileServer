# Stage 1: Build the application
# Defines the .NET SDK version to use for building the application.
# Using alpine for a smaller base image.
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app

# Copy the .csproj file and restore dependencies first.
# This leverages Docker layer caching. If only code changes,
# these layers won't be rebuilt.
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . .

# Publish the application, creating a release build
# Using a specific RID for alpine-x64 if your app isn't self-contained for any-RID,
# but for generic web apps, this is often fine.
# Consider adding --self-contained true -r alpine-x64 if needed.
RUN dotnet publish -c Release -o out

# Stage 2: Create the runtime image
# Defines the .NET runtime version to use. It's smaller than the SDK.
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/out ./

# Expose the port the application listens on.
# Common ports for ASP.NET Core are 80 (HTTP) and 443 (HTTPS).
# Cloud Run will inject its own PORT environment variable, which ASP.NET Core
# automatically picks up by default. So, while it's good practice to
# document the intended port, the app should adapt to Cloud Run's PORT.
EXPOSE 8080

# Define the entry point for the container.
# This command will run when the container starts.
# Replace 'TextFileServer.dll' if your project's output assembly has a different name.
ENTRYPOINT ["dotnet", "TextFileServer.dll"]
