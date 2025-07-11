FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all .csproj files and the solution file
COPY RhyoliteOrbit.sln .
COPY src/RhyoliteOrbit.Application/RhyoliteOrbit.Application.csproj src/RhyoliteOrbit.Application/
COPY src/RhyoliteOrbit.Core/RhyoliteOrbit.Core.csproj src/RhyoliteOrbit.Core/
COPY src/RhyoliteOrbit.EntityFrameworkCore/RhyoliteOrbit.EntityFrameworkCore.csproj src/RhyoliteOrbit.EntityFrameworkCore/
COPY src/RhyoliteOrbit.Migrator/RhyoliteOrbit.Migrator.csproj src/RhyoliteOrbit.Migrator/
COPY src/RhyoliteOrbit.Web.Core/RhyoliteOrbit.Web.Core.csproj src/RhyoliteOrbit.Web.Core/
COPY src/RhyoliteOrbit.Web.Host/RhyoliteOrbit.Web.Host.csproj src/RhyoliteOrbit.Web.Host/
COPY src/RhyoliteOrbit.Web.Mvc/RhyoliteOrbit.Web.Mvc.csproj src/RhyoliteOrbit.Web.Mvc/
COPY test/RhyoliteOrbit.Tests/RhyoliteOrbit.Tests.csproj test/RhyoliteOrbit.Tests/
COPY test/RhyoliteOrbit.Web.Tests/RhyoliteOrbit.Web.Tests.csproj test/RhyoliteOrbit.Web.Tests/

# Restore dependencies for the entire solution
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Publish the Web.Host project
WORKDIR /src/src/RhyoliteOrbit.Web.Host
RUN dotnet publish -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RhyoliteOrbit.Web.Host.dll"]