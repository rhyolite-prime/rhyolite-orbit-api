FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
RUN ln -s ~/.dotnet/tools/libman /usr/local/bin/libman

WORKDIR /src
COPY ["src/RhyoliteOrbit.Web.Mvc/RhyoliteOrbit.Web.Mvc.csproj", "src/RhyoliteOrbit.Web.Mvc/"]
COPY ["src/RhyoliteOrbit.Web.Core/RhyoliteOrbit.Web.Core.csproj", "src/RhyoliteOrbit.Web.Core/"]
COPY ["src/RhyoliteOrbit.Application/RhyoliteOrbit.Application.csproj", "src/RhyoliteOrbit.Application/"]
COPY ["src/RhyoliteOrbit.Core/RhyoliteOrbit.Core.csproj", "src/RhyoliteOrbit.Core/"]
COPY ["src/RhyoliteOrbit.EntityFrameworkCore/RhyoliteOrbit.EntityFrameworkCore.csproj", "src/RhyoliteOrbit.EntityFrameworkCore/"]
WORKDIR "/src/src/RhyoliteOrbit.Web.Mvc"
RUN dotnet restore 

WORKDIR /src
COPY ["src/RhyoliteOrbit.Web.Mvc", "src/RhyoliteOrbit.Web.Mvc"]
COPY ["src/RhyoliteOrbit.Web.Core", "src/RhyoliteOrbit.Web.Core"]
COPY ["src/RhyoliteOrbit.Application", "src/RhyoliteOrbit.Application"]
COPY ["src/RhyoliteOrbit.Core", "src/RhyoliteOrbit.Core"]
COPY ["src/RhyoliteOrbit.EntityFrameworkCore", "src/RhyoliteOrbit.EntityFrameworkCore"]
WORKDIR "/src/src/RhyoliteOrbit.Web.Mvc"
RUN libman restore
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "RhyoliteOrbit.Web.Mvc.dll"]
