FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
ARG MAIN_ROOT
WORKDIR /App

# Copy everything
COPY ../src/${MAIN_ROOT}/ ./${MAIN_ROOT}/
COPY ../src/UOC.SharedContracts/ ./UOC.SharedContracts/

# Restore as distinct layers
RUN dotnet restore ${MAIN_ROOT}/${MAIN_ROOT}.csproj

# Copy sharing settings
WORKDIR /src
COPY ../src/appsettings-shared.json ./appsettings-shared.json

# Build and publish a release
WORKDIR /App
RUN dotnet publish ${MAIN_ROOT} -c Release -o out /p:DebugType=None /p:DebugSymbols=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
ARG MAIN_ROOT
ENV ASSEMBLY=${MAIN_ROOT}.dll
WORKDIR /
COPY --from=build-env /src/appsettings-shared.json .
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT [ "/bin/sh", "-c", "dotnet ${ASSEMBLY}" ]