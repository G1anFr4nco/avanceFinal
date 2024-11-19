using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Consumer
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static Usuario currentUser = null; // Usuario actual (primer usuario recibido)

        static async Task Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                GroupId = "music-preferences-group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("music-preferences");

            Console.WriteLine("Esperando datos de Kafka...");

            try
            {
                // Diccionarios para almacenar el conteo de músicas, artistas y géneros
                var musicasCount = new Dictionary<string, int>();
                var artistasCount = new Dictionary<string, int>();
                var generosCount = new Dictionary<string, int>();

                while (true)
                {
                    var consumeResult = consumer.Consume(CancellationToken.None);
                    var usuario = JsonSerializer.Deserialize<Usuario>(consumeResult.Message.Value);

                    // Asignar el primer usuario como "usuario actual"
                    if (currentUser == null)
                    {
                        currentUser = usuario;
                        Console.WriteLine($"Usuario actual asignado: {JsonSerializer.Serialize(currentUser)}");
                    }

                    // Actualizar el conteo para cada música, artista y género que le gusta al usuario
                    foreach (var musica in usuario.Musicas)
                    {
                        if (musicasCount.ContainsKey(musica))
                            musicasCount[musica]++;
                        else
                            musicasCount[musica] = 1;
                    }

                    foreach (var artista in usuario.Artistas)
                    {
                        if (artistasCount.ContainsKey(artista))
                            artistasCount[artista]++;
                        else
                            artistasCount[artista] = 1;
                    }

                    foreach (var genero in usuario.Generos)
                    {
                        if (generosCount.ContainsKey(genero))
                            generosCount[genero]++;
                        else
                            generosCount[genero] = 1;
                    }

                    // Obtener el top 5 de cada categoría
                    var topMusicas = musicasCount.OrderByDescending(x => x.Value).Take(5)
                        .Select(x => new { nombre = x.Key, conteo = x.Value }).ToList();
                    var topArtistas = artistasCount.OrderByDescending(x => x.Value).Take(5)
                        .Select(x => new { nombre = x.Key, conteo = x.Value }).ToList();
                    var topGeneros = generosCount.OrderByDescending(x => x.Value).Take(5)
                        .Select(x => new { nombre = x.Key, conteo = x.Value }).ToList();

                    // Cálculo de tops relacionados para el usuario actual
                    var userGenres = new HashSet<string>(currentUser.Generos);
                    var userArtists = new HashSet<string>(currentUser.Artistas);
                    var userMusicas = new HashSet<string>(currentUser.Musicas);

                    // Relacionar géneros y artistas
                    var relatedSongs = usuario.Musicas
                        .Where(musica => userGenres.Any(genero => usuario.Generos.Contains(genero)) && !userMusicas.Contains(musica)) // Filtrar música que ya está en los gustos
                        .GroupBy(musica => musica)
                        .Select(g => new { nombre = g.Key, conteo = g.Count() })
                        .OrderByDescending(x => x.conteo)
                        .Take(5)
                        .ToList();

                    var relatedArtists = usuario.Artistas
                        .Where(artista => userGenres.Any(genero => usuario.Generos.Contains(genero)) && !userArtists.Contains(artista)) // Filtrar artistas que ya están en los gustos
                        .GroupBy(artista => artista)
                        .Select(g => new { nombre = g.Key, conteo = g.Count() })
                        .OrderByDescending(x => x.conteo)
                        .Take(5)
                        .ToList();

                    var artistSongs = usuario.Musicas
                        .Where(musica => userArtists.Any(artista => usuario.Artistas.Contains(artista)) && !userMusicas.Contains(musica)) // Filtrar canciones de artistas ya escuchados
                        .GroupBy(musica => musica)
                        .Select(g => new { nombre = g.Key, conteo = g.Count() })
                        .OrderByDescending(x => x.conteo)
                        .Take(5)
                        .ToList();

                    // Crear el objeto con los nuevos datos a enviar
                    var dataToSend = new
                    {
                        currentUser = currentUser, // Usuario actual
                        topData = new
                        {
                            musicas = topMusicas,
                            artistas = topArtistas,
                            generos = topGeneros
                        },
                        relatedData = new
                        {
                            relatedSongs = relatedSongs,
                            relatedArtists = relatedArtists,
                            artistSongs = artistSongs
                        }
                    };

                    // Enviar los datos al servidor Node.js
                    var json = JsonSerializer.Serialize(dataToSend);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    await client.PostAsync("http://localhost:3000/update", content);

                    Console.WriteLine("Datos enviados al servidor Node.js");

                    Thread.Sleep(1000);  // Retardo de 1 segundo entre cada usuario
                }

            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
        }
    }

    public class Usuario
    {
        public string Nombre { get; set; } = string.Empty; // Inicialización segura
        public List<string> Musicas { get; set; } = new List<string>();
        public List<string> Artistas { get; set; } = new List<string>();
        public List<string> Generos { get; set; } = new List<string>();
    }
}
