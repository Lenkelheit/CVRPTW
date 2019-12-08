FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /output

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY API/ source/
COPY ./Shared/ Shared/
WORKDIR /src/source
RUN dotnet publish -c Release -o output

FROM base AS final
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
     && rm -rf /var/lib/apt/lists/*
COPY --from=build /src/source/API/output .
ENTRYPOINT ["dotnet", "API.dll"]