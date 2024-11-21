# Usa una imagen oficial de Node.js como base
FROM node:16

# Crea el directorio de trabajo
WORKDIR /app

# Copia package.json y package-lock.json (si existe)
COPY node/package.json /app/package.json
COPY node/package-lock.json /app/package-lock.json

# Instala las dependencias
RUN npm install

# Copia el resto de los archivos de la aplicación al contenedor
COPY node/ /app/

# Expón el puerto en el que la aplicación escuchará
EXPOSE 3000

# Comando para ejecutar la aplicación
CMD ["npm", "start"]
