# Build image from this Dockerfile using the command
# docker build -t <image name>:<image version> --no-cache  .
# ex
# docker build -t sweetimage:1.3 --no-cache  .

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS publish

WORKDIR /src
COPY ["src/", ""]

RUN dotnet publish "DataCatalog.DatasetResourceManagement/DataCatalog.DatasetResourceManagement.csproj" -c Release -o /app/publish

FROM base
COPY --from=publish /app/publish .
ENV project="DataCatalog.DatasetResourceManagement"
ENTRYPOINT /app/DataCatalog.DatasetResourceManagement
