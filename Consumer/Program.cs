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

                    // Crear el objeto con el top de cada categoría
                    var topData = new
                    {
                        musicas = topMusicas,
                        artistas = topArtistas,
                        generos = topGeneros
                    };

                    // Enviar el topData al servidor Node.js
                    var json = JsonSerializer.Serialize(topData);
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
        public string Nombre { get; set; }
        public List<string> Musicas { get; set; }
        public List<string> Artistas { get; set; }
        public List<string> Generos { get; set; }
    }
}
