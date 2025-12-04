FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY kurssienhallinta/kurssienhallinta.csproj kurssienhallinta/
RUN dotnet restore kurssienhallinta/kurssienhallinta.csproj

COPY kurssienhallinta/. ./kurssienhallinta/
WORKDIR /app/kurssienhallinta
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
EXPOSE 5226
ENV ASPNETCORE_URLS=http://+:5226
ENTRYPOINT ["dotnet", "kurssienhallinta.dll"]
