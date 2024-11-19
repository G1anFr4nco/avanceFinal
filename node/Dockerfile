# Usar la imagen base oficial de Node.js
FROM node:18

# Establecer el directorio de trabajo
WORKDIR /usr/src/app

# Copiar archivos package.json y package-lock.json (si existe)
COPY package*.json ./

# Instalar las dependencias
RUN npm install

# Copiar el resto de los archivos
COPY . .

# Exponer el puerto
EXPOSE 3000

# Comando para iniciar la aplicación
CMD ["node", "server.js"]
