#!/bin/bash

# Inicia el modo Swarm
docker swarm init

# Crea la red overlay
docker network create --driver=overlay app-network

# Construye las imágenes
docker-compose build

# Despliega la aplicación
docker stack deploy -c docker-compose.yml music-stack
