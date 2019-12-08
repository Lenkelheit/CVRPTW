FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /output

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY OR-Tools/ source/
COPY ./Shared/ Shared/
WORKDIR /src/source
RUN dotnet publish -c Release -o output

FROM base AS final
COPY --from=build /src/source/OR-Tools/output .
ENTRYPOINT ["dotnet", "OR-Tools.dll"]