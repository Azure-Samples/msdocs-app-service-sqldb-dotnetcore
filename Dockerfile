FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DotNetCoreSqlDb.csproj ./
RUN dotnet restore

COPY . ./

# Build EF migration bundle for container startup migration.
RUN dotnet tool install --global dotnet-ef --version 8.0.6
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet ef migrations bundle --target-runtime linux-x64 --force -o /src/migrationsbundle

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish ./
COPY --from=build /src/migrationsbundle ./migrationsbundle
COPY docker-entrypoint.sh /app/docker-entrypoint.sh

RUN chmod +x /app/docker-entrypoint.sh /app/migrationsbundle

EXPOSE 8080

ENTRYPOINT ["/app/docker-entrypoint.sh"]
