#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/windows/servercore:20H2-amd64 AS base
ENV ASPNETCORE_URLS=https://+:443
ENV ASPNETCORE_ENVIRONMENT Development
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=admin1234
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/root/.aspnet/https/PublicApi.pfx
      
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/windows/servercore:20H2-amd64  AS build
WORKDIR /app
COPY . .
#COPY ["src/PublicApi/PublicApi.csproj", "./PublicApi/"]
#RUN dotnet restore "./PublicApi/PublicApi.csproj"
#COPY . .
WORKDIR "/app/src/PublicApi"
RUN dotnet restore

RUN dotnet build "./PublicApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./PublicApi.csproj" -c Release -o /app/publish

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT Development
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PublicApi.dll"]
