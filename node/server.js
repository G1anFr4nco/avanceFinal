const express = require('express');
const http = require('http');
const socketIo = require('socket.io');

const app = express();
const server = http.createServer(app);
const io = socketIo(server);

const PORT = 3000;

// Servir la carpeta pública
app.use(express.static('public'));

// Endpoint para recibir datos desde el consumidor de Kafka en C#
app.use(express.json());

app.post('/update', (req, res) => {
    const data = req.body;
    io.emit('updateData', data);  // Enviar datos al frontend
    res.status(200).send('Data received');
});

server.listen(PORT, () => {
    console.log(`Servidor escuchando en el puerto ${PORT}`);
});
