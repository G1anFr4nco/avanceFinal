const socket = io();

// Función para renderizar los tops
function renderTop(id, data) {
  const container = document.getElementById(id);
  container.innerHTML = '';
  data.forEach(item => {
    const card = document.createElement('div');
    card.classList.add('top-card');
    card.innerHTML = `
      <img src="https://via.placeholder.com/150" alt="${item.nombre}">
      <h5>${item.nombre}</h5>
      <p>${item.conteo} veces</p>
    `;
    container.appendChild(card);
  });
}

// Renderizar datos del usuario actual
function renderUsuario(data) {
  document.getElementById('usuarioNombre').textContent = `Nombre: ${data.Nombre}`;
  document.getElementById('usuarioMusicas').textContent = data.Musicas.join(', ');
  document.getElementById('usuarioArtistas').textContent = data.Artistas.join(', ');
  document.getElementById('usuarioGeneros').textContent = data.Generos.join(', ');
}

// Escuchar los datos en tiempo real
socket.on('updateData', data => {
  renderUsuario(data.currentUser);
  renderTop('musicasTop', data.topData.musicas);
  renderTop('artistasTop', data.topData.artistas);
  renderTop('generosTop', data.topData.generos);
});

// Obtener el token de Spotify y luego las canciones
async function fetchSpotifyToken() {
  try {
    const clientId = '2919bca9c33d49389ca8145fdf456fd2';
    const clientSecret = 'dca1d5663ed34482b2670e2b2a041c0b';
    const response = await fetch('https://accounts.spotify.com/api/token', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
        'Authorization': 'Basic ' + btoa(clientId + ':' + clientSecret)
      },
      body: 'grant_type=client_credentials'
    });
    if (!response.ok) {
      throw new Error('Error al obtener el token');
    }
    const data = await response.json();
    return data.access_token;
  } catch (error) {
    console.error('Error al obtener el token de Spotify:', error);
  }
}

async function getSpotifyTracks() {
  const token = await fetchSpotifyToken();
  if (!token) return;

  try {
    const response = await fetch('https://api.spotify.com/v1/browse/featured-playlists', {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });
    if (!response.ok) {
      throw new Error('Error al obtener canciones de Spotify');
    }
    const data = await response.json();
    console.log(data); // Puedes usar los datos como lo necesites
  } catch (error) {
    console.error('Error al obtener las canciones de Spotify:', error);
  }
}

getSpotifyTracks();
