const express = require('express');
const http = require('http');
const socketIo = require('socket.io');
require('dotenv').config();

const app = express();
const server = http.createServer(app);
const io = socketIo(server);

const PORT = process.env.PORT || 3000;

// Servir la carpeta pública
app.use(express.static('public'));

// Middleware para parsear JSON
app.use(express.json());

// Endpoint para recibir datos desde el consumidor de Kafka en C#
app.post('/update', (req, res) => {
    const data = req.body;

    // Validar si el cuerpo de la solicitud contiene los datos esperados
    if (!data || !data.currentUser || !data.topData) {
        return res.status(400).send('Datos inválidos');
    }

    // Emitir los datos al frontend mediante WebSocket
    io.emit('updateData', data); // Enviar datos al frontend

    res.status(200).send('Datos recibidos y enviados al cliente');
});

// Configuración del servidor para escuchar el puerto
server.listen(PORT, '0.0.0.0',() => {
    console.log(`Servidor escuchando en el puerto ${PORT}`);
});
