FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy everything else and build
COPY . ./
RUN dotnet restore "./CsvToVcfConverter/CsvToVcfConverter.csproj"
RUN dotnet publish "./CsvToVcfConverter/CsvToVcfConverter.csproj" -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production

# Let the runtime bind to the port provided by the platform via $PORT
# If $PORT is not set, fall back to 80
ENV PORT=80
EXPOSE 80

# Use shell form to expand $PORT at container runtime so the app listens
# on the port Railway expects (via the PORT env var).
ENTRYPOINT ["/bin/sh","-c","exec dotnet CsvToVcfConverter.dll --urls http://0.0.0.0:${PORT}"]