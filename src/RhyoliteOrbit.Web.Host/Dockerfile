FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/RhyoliteOrbit.Web.Host/RhyoliteOrbit.Web.Host.csproj", "src/RhyoliteOrbit.Web.Host/"]
COPY ["src/RhyoliteOrbit.Web.Core/RhyoliteOrbit.Web.Core.csproj", "src/RhyoliteOrbit.Web.Core/"]
COPY ["src/RhyoliteOrbit.Application/RhyoliteOrbit.Application.csproj", "src/RhyoliteOrbit.Application/"]
COPY ["src/RhyoliteOrbit.Core/RhyoliteOrbit.Core.csproj", "src/RhyoliteOrbit.Core/"]
COPY ["src/RhyoliteOrbit.EntityFrameworkCore/RhyoliteOrbit.EntityFrameworkCore.csproj", "src/RhyoliteOrbit.EntityFrameworkCore/"]
WORKDIR "/src/src/RhyoliteOrbit.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/RhyoliteOrbit.Web.Host", "src/RhyoliteOrbit.Web.Host"]
COPY ["src/RhyoliteOrbit.Web.Core", "src/RhyoliteOrbit.Web.Core"]
COPY ["src/RhyoliteOrbit.Application", "src/RhyoliteOrbit.Application"]
COPY ["src/RhyoliteOrbit.Core", "src/RhyoliteOrbit.Core"]
COPY ["src/RhyoliteOrbit.EntityFrameworkCore", "src/RhyoliteOrbit.EntityFrameworkCore"]
WORKDIR "/src/src/RhyoliteOrbit.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "RhyoliteOrbit.Web.Host.dll"]
