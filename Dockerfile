FROM mcr.microsoft.com/dotnet/sdk:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN dotnet tool install --global dotnet-format
RUN apt-get install git

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CodeGenerator.csproj", "."]
RUN dotnet restore "./CodeGenerator.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CodeGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeGenerator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV PATH="${PATH}:/root/.dotnet/tools" \
    ASPNETCORE_URLS=http://+:80 
ENTRYPOINT ["dotnet", "CodeGenerator.dll"]