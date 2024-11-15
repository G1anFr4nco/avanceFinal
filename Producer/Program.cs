using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Producer
{
    class Program
    {
        private static readonly List<string> Musicas = new List<string> { "Shape of You", "Blinding Lights", "Someone You Loved", "Bad Guy", "Old Town Road" };
        private static readonly List<string> Artistas = new List<string> { "Ed Sheeran", "The Weeknd", "Lewis Capaldi", "Billie Eilish", "Lil Nas X" };
        private static readonly List<string> Generos = new List<string> { "Pop", "Hip-Hop", "Rock", "Country", "Electronic" };

        static async Task Main(string[] args)
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
            using var producer = new ProducerBuilder<Null, string>(config).Build();

            while (true)
            {
                var usuario = GenerarDatosUsuario();
                var jsonData = JsonSerializer.Serialize(usuario);

                await producer.ProduceAsync("music-preferences", new Message<Null, string> { Value = jsonData });
                Console.WriteLine($"Datos enviados a Kafka: {jsonData}");

                await Task.Delay(1000); // Espera de 1 segundo entre cada usuario
            }
        }

        private static dynamic GenerarDatosUsuario()
        {
            var random = new Random();
            return new
            {
                Nombre = $"Usuario_{Guid.NewGuid().ToString().Substring(0, 8)}",
                Musicas = ObtenerAleatorio(Musicas, random),
                Artistas = ObtenerAleatorio(Artistas, random),
                Generos = ObtenerAleatorio(Generos, random)
            };
        }

        private static List<string> ObtenerAleatorio(List<string> lista, Random random)
        {
            int cantidad = random.Next(1, lista.Count);
            var seleccionados = new List<string>();

            while (seleccionados.Count < cantidad)
            {
                var item = lista[random.Next(lista.Count)];
                if (!seleccionados.Contains(item))
                {
                    seleccionados.Add(item);
                }
            }
            return seleccionados;
        }
    }
}
