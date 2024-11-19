using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Producer
{
    class Program
    {
        // Relación directa entre canciones, artistas y géneros
        private static readonly List<(string Cancion, string Artista, string Genero)> MusicasRelaciones = new List<(string, string, string)>
        {
            ("Despacito", "Luis Fonsi", "Reguetón"),
            ("La Bicicleta", "Carlos Vives y Shakira", "Vallenato"), //
            ("Felices los 4", "Maluma", "Reguetón"),
            ("Bailando", "Enrique Iglesias", "Pop Latino"), //
            ("Chantaje", "Shakira", "Reguetón"),
            ("Tusa", "Karol G y Nicki Minaj", "Reguetón"),
            ("Vivir mi Vida", "Marc Anthony", "Salsa"),
            ("Échame la Culpa", "Luis Fonsi y Demi Lovato", "Pop Latino"),
            ("Mi Gente", "J Balvin", "Reguetón"),
            ("El Perdón", "Nicky Jam y Enrique Iglesias", "Reguetón"),
            ("Provenza", "Karol G", "Pop Urbano"),
            ("Te Boté", "Bad Bunny y Ozuna", "Trap Latino"),
            ("Hawái", "Maluma", "Pop Urbano"),
            ("Corazón Partío", "Alejandro Sanz", "Balada"),
            ("La Gozadera", "Gente de Zona y Marc Anthony", "Salsa") //
        };

        private static readonly List<(string Artista, string Genero)> ArtistasRelaciones = new List<(string, string)>
        {
            ("Luis Fonsi", "Reguetón"),
            ("Carlos Vives", "Vallenato"),
            ("Shakira", "Pop Latino"),
            ("Maluma", "Reguetón"),
            ("Enrique Iglesias", "Pop Latino"),
            ("Karol G", "Reguetón"),
            ("Marc Anthony", "Salsa"),
            ("J Balvin", "Reguetón"),
            ("Nicky Jam", "Reguetón"),
            ("Bad Bunny", "Trap Latino"),
            ("Ozuna", "Trap Latino"),
            ("Alejandro Sanz", "Balada"),
            ("Gente de Zona", "Salsa")
        };

        private static readonly List<string> Generos = new List<string> 
        {
            "Reguetón",
            "Vallenato",
            "Pop Latino",
            "Salsa",
            "Trap Latino",
            "Balada",
            "Pop Urbano"
        };

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
            var musicasSeleccionadas = ObtenerAleatorio(MusicasRelaciones, random, 3); // Limitar a 3 canciones

            // Separar canciones, artistas y géneros seleccionados
            var canciones = new List<string>();
            var artistas = new List<string>();
            var generos = new List<string>();

            foreach (var musica in musicasSeleccionadas)
            {
                canciones.Add(musica.Cancion);
                if (!artistas.Contains(musica.Artista))
                    artistas.Add(musica.Artista);
                if (!generos.Contains(musica.Genero))
                    generos.Add(musica.Genero);
            }

            return new
            {
                Nombre = $"Usuario_{Guid.NewGuid().ToString().Substring(0, 8)}",
                Musicas = canciones,
                Artistas = artistas,
                Generos = generos
            };
        }

        // Método para obtener selecciones aleatorias, con un límite de elementos
        private static List<(string Cancion, string Artista, string Genero)> ObtenerAleatorio(List<(string Cancion, string Artista, string Genero)> lista, Random random, int limite)
        {
            var seleccionados = new List<(string Cancion, string Artista, string Genero)>();
            while (seleccionados.Count < limite)
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
