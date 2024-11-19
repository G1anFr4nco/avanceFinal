# Usar una imagen base de .NET
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Usar la imagen de SDK de .NET para la construcción
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiar el archivo Consumer.csproj desde el directorio de Consumer
COPY Consumer/Consumer.csproj ./Consumer/

# Restaurar las dependencias
RUN dotnet restore "Consumer/Consumer.csproj"

# Copiar el resto del código fuente
COPY . ./
WORKDIR /src/Consumer

# Construir el proyecto
RUN dotnet build "Consumer.csproj" -c Release -o /app/build

# Publicar el proyecto
FROM build AS publish
RUN dotnet publish "Consumer.csproj" -c Release -o /app/publish

# Imagen final con los archivos publicados
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consumer.dll"]
