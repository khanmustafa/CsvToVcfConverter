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
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80
ENTRYPOINT ["dotnet", "CsvToVcfConverter.dll"]