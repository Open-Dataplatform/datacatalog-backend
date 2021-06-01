# Build image from this Dockerfile using the command
# docker build -t <image name>:<image version> --no-cache  . --build-arg source_folder=<source folder> --build-arg project=<project>
# ex
# docker build -t sweetimage:1.3 --no-cache  . --build-arg source_folder=src/DataCatalog.Api/ --build-arg project=DataCatalog.Api
#
# This file assumes the following:
# 1) Only one project needs building
# 2) The assembly name is project name appended with .dll

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS publish

ARG source_folder
ARG project

WORKDIR /src
COPY ["$source_folder", ""]

RUN dotnet publish "$project.csproj" -c Release -o /app/publish

FROM base
ARG project
COPY --from=publish /app/publish .
ENV project=$project
ENTRYPOINT /app/$project
