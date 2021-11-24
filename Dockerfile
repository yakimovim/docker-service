FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

COPY . /app/

WORKDIR /app

RUN ["dotnet", "tool", "restore"]

RUN ["dotnet", "cake"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

COPY --from=build /app/output /app/

WORKDIR /app

EXPOSE 80

ENTRYPOINT ["dotnet", "Service.dll"]