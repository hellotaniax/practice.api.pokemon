using Newtonsoft.Json;
using PokemonReviewApp.Dto;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PokemonReviewApp.ConsoleTests
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string baseUrl = "https://localhost:7066/api/Pokemons"; // Ajusta la URL según tu configuración

        static async Task Main(string[] args)
        {
            Console.WriteLine("Pokemon API CRUD Tests");
            Console.WriteLine("=====================");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                await RunAllTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }

            Console.WriteLine("\nPruebas completadas. Presiona cualquier tecla para salir.");
            Console.ReadKey();
        }

        static async Task RunAllTests()
        {
            // GET - Obtener todos los Pokemon
            await GetAllPokemons();

            // POST - Crear un nuevo Pokemon
            int newPokemonId = await CreatePokemon();

            if (newPokemonId > 0)
            {
                // GET - Obtener un Pokemon específico
                await GetPokemonById(newPokemonId);

                // GET - Obtener calificación de Pokemon
                await GetPokemonRating(newPokemonId);

                // PUT - Actualizar un Pokemon
                await UpdatePokemon(newPokemonId);

                // DELETE - Eliminar un Pokemon
                await DeletePokemon(newPokemonId);

                // Verificar que se eliminó
                await VerifyPokemonDeleted(newPokemonId);
            }
        }

        static async Task GetAllPokemons()
        {
            Console.WriteLine("\n[TEST] Obteniendo todos los Pokemon...");
            HttpResponseMessage response = await client.GetAsync(baseUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var pokemons = JsonConvert.DeserializeObject<List<PokemonDto>>(responseBody);

                Console.WriteLine($"✅ ÉXITO: Se encontraron {pokemons.Count} Pokemon.");
                Console.WriteLine("Primeros 3 Pokemon:");

                int count = 0;
                foreach (var pokemon in pokemons)
                {
                    if (count++ >= 3) break;
                    Console.WriteLine($"  - ID: {pokemon.Id}, Nombre: {pokemon.Name}");
                }
            }
            else
            {
                Console.WriteLine($"❌ ERROR: No se pudieron obtener los Pokemon. Código: {response.StatusCode}");
            }
        }

        static async Task<int> CreatePokemon()
        {
            Console.WriteLine("\n[TEST] Creando un nuevo Pokemon...");

            // Generar un nombre único para evitar conflictos
            string uniqueName = $"TestPokemon_{DateTime.Now.Ticks}";

            var newPokemon = new
            {
                Name = uniqueName,
                BirthDate = DateTime.Now
            };

            int ownerId = 1; // Asegúrate de que este ID exista en tu base de datos
            int categoryId = 1; // Asegúrate de que este ID exista en tu base de datos

            var content = new StringContent(
                JsonConvert.SerializeObject(newPokemon),
                Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await client.PostAsync($"{baseUrl}?ownerId={ownerId}&categoryId={categoryId}", content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ ÉXITO: Pokemon creado. Respuesta: {responseBody}");

                // Ahora necesitamos obtener el ID del Pokemon recién creado
                // Buscar por nombre en la lista de todos los Pokemon
                var getAllResponse = await client.GetAsync(baseUrl);
                if (getAllResponse.IsSuccessStatusCode)
                {
                    var allPokemonsContent = await getAllResponse.Content.ReadAsStringAsync();
                    var allPokemons = JsonConvert.DeserializeObject<List<PokemonDto>>(allPokemonsContent);

                    var createdPokemon = allPokemons.Find(p => p.Name == uniqueName);
                    if (createdPokemon != null)
                    {
                        Console.WriteLine($"✅ Pokemon creado con ID: {createdPokemon.Id}, Nombre: {createdPokemon.Name}");
                        return createdPokemon.Id;
                    }
                }

                Console.WriteLine("⚠️ No se pudo determinar el ID del Pokemon creado.");
                return -1;
            }
            else
            {
                Console.WriteLine($"❌ ERROR: No se pudo crear el Pokemon. Código: {response.StatusCode}");
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Detalle: {responseBody}");
                return -1;
            }
        }

        static async Task GetPokemonById(int pokemonId)
        {
            Console.WriteLine($"\n[TEST] Obteniendo Pokemon con ID {pokemonId}...");

            HttpResponseMessage response = await client.GetAsync($"{baseUrl}/{pokemonId}");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var pokemon = JsonConvert.DeserializeObject<PokemonDto>(responseBody);

                Console.WriteLine($"✅ ÉXITO: Pokemon encontrado - ID: {pokemon.Id}, Nombre: {pokemon.Name}");
            }
            else
            {
                Console.WriteLine($"❌ ERROR: No se pudo obtener el Pokemon. Código: {response.StatusCode}");
            }
        }

        static async Task GetPokemonRating(int pokemonId)
        {
            Console.WriteLine($"\n[TEST] Obteniendo calificación del Pokemon con ID {pokemonId}...");

            HttpResponseMessage response = await client.GetAsync($"{baseUrl}/{pokemonId}/rating");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var rating = JsonConvert.DeserializeObject<decimal>(responseBody);

                Console.WriteLine($"✅ ÉXITO: La calificación del Pokemon es: {rating}");
            }
            else
            {
                Console.WriteLine($"❌ ERROR: No se pudo obtener la calificación. Código: {response.StatusCode}");
            }
        }

        static async Task UpdatePokemon(int pokemonId)
        {
            Console.WriteLine($"\n[TEST] Actualizando Pokemon con ID {pokemonId}...");

            // Primero, obtener el Pokemon actual
            HttpResponseMessage getResponse = await client.GetAsync($"{baseUrl}/{pokemonId}");

            if (getResponse.IsSuccessStatusCode)
            {
                string getResponseBody = await getResponse.Content.ReadAsStringAsync();
                var pokemon = JsonConvert.DeserializeObject<PokemonDto>(getResponseBody);

                // Modificar el nombre
                string updatedName = $"Updated_{pokemon.Name}";
                pokemon.Name = updatedName;

                var content = new StringContent(
                    JsonConvert.SerializeObject(pokemon),
                    Encoding.UTF8,
                    "application/json");

                HttpResponseMessage putResponse = await client.PutAsync($"{baseUrl}/{pokemonId}", content);

                if (putResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    Console.WriteLine($"✅ ÉXITO: Pokemon actualizado a '{updatedName}'");

                    // Verificar la actualización
                    HttpResponseMessage verifyResponse = await client.GetAsync($"{baseUrl}/{pokemonId}");
                    string verifyResponseBody = await verifyResponse.Content.ReadAsStringAsync();
                    var updatedPokemon = JsonConvert.DeserializeObject<PokemonDto>(verifyResponseBody);

                    if (updatedPokemon.Name == updatedName)
                    {
                        Console.WriteLine("✅ VERIFICADO: El nombre se actualizó correctamente.");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ ADVERTENCIA: La verificación falló. Nombre actual: {updatedPokemon.Name}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ ERROR: No se pudo actualizar el Pokemon. Código: {putResponse.StatusCode}");
                    string responseBody = await putResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Detalle: {responseBody}");
                }
            }
            else
            {
                Console.WriteLine($"❌ ERROR: No se pudo obtener el Pokemon para actualizar. Código: {getResponse.StatusCode}");
            }
        }

        static async Task DeletePokemon(int pokemonId)
        {
            Console.WriteLine($"\n[TEST] Eliminando Pokemon con ID {pokemonId}...");

            HttpResponseMessage response = await client.DeleteAsync($"{baseUrl}/{pokemonId}");

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                Console.WriteLine("✅ ÉXITO: Pokemon eliminado correctamente.");
            }
            else
            {
                Console.WriteLine($"❌ ERROR: No se pudo eliminar el Pokemon. Código: {response.StatusCode}");
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Detalle: {responseBody}");
            }
        }

        static async Task VerifyPokemonDeleted(int pokemonId)
        {
            Console.WriteLine($"\n[TEST] Verificando que el Pokemon con ID {pokemonId} fue eliminado...");

            HttpResponseMessage response = await client.GetAsync($"{baseUrl}/{pokemonId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("✅ VERIFICADO: El Pokemon ya no existe (NotFound).");
            }
            else
            {
                Console.WriteLine($"⚠️ ADVERTENCIA: La verificación de eliminación falló. Código: {response.StatusCode}");
            }
        }
    }

    // Definición mínima de PokemonDto para deserialización
    public class PokemonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }
}