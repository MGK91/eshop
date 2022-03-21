#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/windows/servercore:20H2-amd64 AS base
ENV ASPNETCORE_URLS=https://+:443
ENV ASPNETCORE_ENVIRONMENT Development
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=admin@123
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/root/.aspnet/https/Web.pfx
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/windows/servercore:20H2-amd64 AS build
WORKDIR /src
COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/BlazorShared/BlazorShared.csproj", "src/BlazorShared/"]
COPY ["src/ApplicationCore/ApplicationCore.csproj", "src/ApplicationCore/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/BlazorAdmin/BlazorAdmin.csproj", "src/BlazorAdmin/"]
RUN dotnet restore "src/Web/Web.csproj"
COPY . .
WORKDIR "/src/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT Development
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]
