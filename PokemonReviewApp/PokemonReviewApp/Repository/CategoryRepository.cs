using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;

        public CategoryRepository(DataContext context)
        {
            _context = context;
        }
        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.Id == id);
        }

        public bool CreateCategory(Category category)
        {
            _context.Add(category); //Agregamos la categoria a la base de datos
            return Save(); //Guardamos los cambios
        }

        public bool DeleteCategory(Category category)
        {
            _context.Remove(category); //Eliminamos la categoria
            return Save(); //Guardamos los cambios
        }

        public ICollection<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.Where(e => e.Id == id).FirstOrDefault();
        }


        public ICollection<Pokemon> GetPokemonByCategory(int categoryId)
        {
            return _context.PokemonCategories.Where(e => e.CategoryId == categoryId).Select(c => c.Pokemon).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false; //Si se guardaron cambios, devuelve true
        }

        public bool UpdateCategory(Category category)
        {
            _context.Update(category); //Actualiza la categoria
            return Save(); //Guarda los cambios
           
        }
    }
}
