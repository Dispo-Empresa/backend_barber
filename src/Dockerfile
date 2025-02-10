FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Dispo.Barber.API/Dispo.Barber.API.csproj", "Dispo.Barber.API/"]
COPY ["Dispo.Barber.Application/Dispo.Barber.Application.csproj", "Dispo.Barber.Application/"]
COPY ["Dispo.Barber.Domain/Dispo.Barber.Domain.csproj", "Dispo.Barber.Domain/"]
COPY ["Dispo.Barber.Infrastructure/Dispo.Barber.Infrastructure.csproj", "Dispo.Barber.Infrastructure/"]
COPY ["Dispo.Barber.Bundle/Dispo.Barber.Bundle.csproj", "Dispo.Barber.Bundle/"]
RUN dotnet restore "Dispo.Barber.API/Dispo.Barber.API.csproj"
COPY . .
WORKDIR "/src/Dispo.Barber.API"
RUN dotnet build "Dispo.Barber.API.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Dispo.Barber.API.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Dispo.Barber.API.dll" ]