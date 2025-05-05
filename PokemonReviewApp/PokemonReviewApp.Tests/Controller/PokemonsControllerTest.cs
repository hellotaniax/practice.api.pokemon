using AutoMapper; // Importa el espacio de nombres de la biblioteca AutoMapper, utilizada para la asignación de objetos.
using FakeItEasy; // Importa el espacio de nombres de la biblioteca FakeItEasy, un framework para crear objetos simulados (fakes) para pruebas.
using PokemonReviewApp.Interfaces; // Importa el espacio de nombres que contiene las interfaces de los repositorios de la aplicación.
using System; // Importa el espacio de nombres System, que proporciona tipos y funcionalidades fundamentales.
using System.Collections.Generic; // Importa el espacio de nombres System.Collections.Generic, que define interfaces y clases que definen colecciones genéricas.
using System.Linq; // Importa el espacio de nombres System.Linq, que proporciona métodos para consultar y manipular colecciones.
using System.Text; // Importa el espacio de nombres System.Text, que proporciona clases para codificar, decodificar, convertir y manipular texto.
using System.Threading.Tasks; // Importa el espacio de nombres System.Threading.Tasks, que proporciona compatibilidad para operaciones asincrónicas.

namespace PokemonReviewApp.Tests.Controller // Declara un espacio de nombres para organizar las clases de prueba relacionadas con los controladores.
{
    public class PokemonsControllerTest // Define una clase interna llamada PokemonsControllerTest, que contendrá las pruebas unitarias para el controlador de Pokémon. La clase es interna, lo que significa que solo es accesible dentro de su propio ensamblado (proyecto de pruebas).
    {
        private readonly IPokemonRepository _pokemonRepository; // Declara un campo privado de solo lectura llamado _pokemonRepository del tipo IPokemonRepository. Se utilizará para almacenar un objeto simulado (fake) del repositorio de Pokémon. 'readonly' asegura que solo se asigne en el constructor.
        private readonly IReviewRepository _reviewRepository; // Declara un campo privado de solo lectura llamado _reviewRepository del tipo IReviewRepository. Se utilizará para almacenar un objeto simulado (fake) del repositorio de reseñas.
        private readonly IMapper _mapper; // Declara un campo privado de solo lectura llamado _mapper del tipo IMapper. Se utilizará para almacenar un objeto simulado (fake) del servicio de AutoMapper.

        public PokemonsControllerTest() // Define el constructor público de la clase PokemonsControllerTest. Este constructor se ejecuta cada vez que se crea una instancia de esta clase de prueba.
        {
            _pokemonRepository = A.Fake<IPokemonRepository>(); // Utiliza el método A.Fake<T>() de FakeItEasy para crear una instancia simulada (fake) de la interfaz IPokemonRepository. Esta instancia fake se utilizará para controlar el comportamiento del repositorio durante las pruebas.
            _reviewRepository = A.Fake<IReviewRepository>(); // Similar a la línea anterior, crea una instancia simulada (fake) de la interfaz IReviewRepository.
            _mapper = A.Fake<IMapper>(); // Crea una instancia simulada (fake) de la interfaz IMapper de AutoMapper.
        }
    }
}