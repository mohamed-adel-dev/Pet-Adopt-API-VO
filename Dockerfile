# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy csproj files
COPY ["PetAdopt.API/PetAdopt.API.csproj", "PetAdopt.API/"]
COPY ["PetAdopt.BLL/PetAdopt.BLL.csproj", "PetAdopt.BLL/"]
COPY ["PetAdopt.DAL/PetAdopt.DAL.csproj", "PetAdopt.DAL/"]

# Restore
RUN dotnet restore "PetAdopt.API/PetAdopt.API.csproj"

# Copy everything
COPY . .

# Publish
RUN dotnet publish "PetAdopt.API/PetAdopt.API.csproj" \
    -c Release \
    -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "PetAdopt.API.dll"]