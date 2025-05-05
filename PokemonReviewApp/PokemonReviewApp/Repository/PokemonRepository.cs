using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository : IPokemonRepository //Implementamos la interfaz
    {
        private readonly DataContext _context; 
        public PokemonRepository(DataContext context) //Tenemos acceso a las tablas de datacontext
        {
            _context = context;
        }

        public bool CreatePokemon(int ownnerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _context.Owners.Where(o => o.Id == ownnerId).FirstOrDefault(); //Buscamos el owner por id
            var category = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault(); //Buscamos la categoria por id

            var pokemonOwner = new PokemonOwner() //Creamos el owner
            {
                Owner = pokemonOwnerEntity, //Asignamos el owner
                Pokemon = pokemon //Asignamos el pokemon
            };
            _context.Add(pokemonOwner); //Agregamos el owner a la base de datos

            var pokemonCategory = new PokemonCategory() //Creamos la categoria
            {
                Category = category, //Asignamos la categoria
                Pokemon = pokemon //Asignamos el pokemon
            };
            _context.Add(pokemonCategory); //Agregamos la categoria a la base de datos
            _context.Add(pokemon); //Agregamos el pokemon a la base de datos

            return Save(); //Guardamos los cambios
        }

        public bool DeletePokemon(Pokemon pokemon)
        {
            _context.Remove(pokemon); //Eliminamos el pokemon
            return Save(); //Guardamos los cambios
        }

        public Pokemon GetPokemon(int id)
        {
            return _context.Pokemons.Where(p => p.Id == id).FirstOrDefault(); //Buscamos el pokemon por id
        }

        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemons.Where(p => p.Name == name).FirstOrDefault(); //Buscamos el pokemon por nombre
        }

        public decimal GetPokemonRating(int pokeId)
        {
            var review = _context.Reviews.Where(r => r.Pokemon.Id == pokeId); //Buscamos las reviews del pokemon
            if (review.Count() <= 0) //Si no hay reviews
                return 0; //Retornamos 0
            return ((decimal)review.Sum(r => r.Rating) / review.Count()); //Retornamos la suma de las reviews entre la cantidad de reviews
        }

        public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemons.OrderBy(p => p.Id).ToList(); //Ordena por id
        }

        public bool PokemonExists(int pokeId)
        {
            return _context.Pokemons.Any(p => p.Id == pokeId); //Verificamos si existe el pokemon
        }

        public bool Save()
        {
            var save = _context.SaveChanges(); //Guardamos los cambios
            return save > 0 ? true : false; //Si se guardaron los cambios retornamos true
        }

        public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            _context.Update(pokemon); //Actualizamos el pokemon
            return Save(); //Guardamos los cambios
        }
    }
}
