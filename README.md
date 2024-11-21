# Kafka Producer-Consumer Example with Docker

Este repositorio contiene un ejemplo de implementación de un productor y consumidor de Kafka utilizando Docker, Docker Compose, y .NET para el consumidor, y Node.js para el productor. Este proyecto está diseñado para facilitar el envío y consumo de mensajes a través de Kafka en un entorno de contenedores.

## Requisitos

Antes de ejecutar este proyecto, asegúrate de que la nueva máquina tenga las siguientes dependencias instaladas:

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Clonar el Repositorio

Clona el repositorio en tu máquina local con el siguiente comando:

```bash
git clone [<url-del-repositorio>](https://github.com/G1anFr4nco/avanceFinal.git)
cd avanceFinal
```
Una vez que hayas clonado el repositorio, navega a la carpeta raíz del proyecto y ejecuta los siguientes comandos para construir y levantar los contenedores:
```bash
docker-compose up --build
```
