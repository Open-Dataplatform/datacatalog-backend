# Build image from this Dockerfile using the command
# docker build -t <image name>:<image version> --no-cache  . --build-arg feed_password=<PAT obtained from wither CredentialProvider or manually> --build-arg source_folder=<source folder> --build-arg project=<project> --build-arg nuget_config=<name of nuget config file>
# ex
# docker build -t sweetimage:1.3 --no-cache  . --build-arg feed_password=rnwywc7olv4royoegshgapsosk5tyaggiiim44ik6nsxo7qlmaga --build-arg source_folder=src/DataCatalog.Api/ --build-arg project=DataCatalog.Api --build-arg nuget_config=NuGet.config
#
# This file assumes the following:
# 1) Only the mentioned feed needs credentials. If several feeds needs credentials expand the arg list and the endpointCredentials object
# 2) A NuGet.config file exist but this normally also the case
# 3) Only one project needs building
# 4) The assembly name is project name appended with .dll

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

ARG feed_password
ARG source_folder
ARG project
ARG nuget_config

ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
ENV NUGET_CREDENTIALPROVIDER_SESSIONTOKENCACHE_ENABLED true
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS {\"endpointCredentials\": [{\"endpoint\":\"https://pkgs.dev.azure.com/energinet/DataPlatform/_packaging/DataPlatform/nuget/v3/index.json\", \"password\":\"$feed_password\"}]}

RUN wget -qO- https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | bash

WORKDIR /src
COPY ["$source_folder", ""]
COPY ["$nuget_config", ""]

RUN dotnet restore "./$project.csproj" --configfile "$nuget_config"
WORKDIR "/src/."
RUN dotnet build "$project.csproj"  -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "$project.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "$project.dll"]
