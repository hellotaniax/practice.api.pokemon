using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) //proporcionada por Entity Framework Core para realizar CRUD con la base
        {

        }
        //Tablas de la base de datos
        public DbSet<Category> Categories { get; set; } 
        public DbSet<Country> Countries { get; set; }
        public DbSet<Pokemon> Pokemons { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<PokemonCategory> PokemonCategories { get; set; }
        public DbSet<PokemonOwner> PokemonOwners { get; set; }
        public DbSet<Owner> Owners { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) //configurar las relaciones
        {
            modelBuilder.Entity<PokemonCategory>()
                .HasKey(pc => new { pc.PokemonId, pc.CategoryId });  //clave primaria para la entidad PokemonCategory.

            // Cada PokemonCategory tiene un Pokemon asociado.
            modelBuilder.Entity<PokemonCategory>() 
                .HasOne(p => p.Pokemon) //navegacion Pokemon en PokemonCategory.
                .WithMany(pc => pc.PokemonCategories) // Especifica la colección de navegación PokemonCategories en Pokemon.
                .HasForeignKey(p => p.PokemonId); // PokemonId como la clave foránea en PokemonCategory que referencia a Pokemon.

            // Cada PokemonCategory tiene una Category asociada.
            modelBuilder.Entity<PokemonCategory>() 
                .HasOne(p => p.Category)//navegacion Category en PokemonCategory.
                .WithMany(pc => pc.PokemonCategories)//espicifica la coleccion de navegación PokemonCategories en Category.
                .HasForeignKey(c => c.CategoryId); //clave foranea en PokemonCategory que referencia a Category.

       
            modelBuilder.Entity<PokemonOwner>()
                .HasKey(po => new { po.PokemonId, po.OwnerId });  //Clave primaria para la entidad PokemonOwner.

            // Cada PokemonOwner tiene un Pokemon asociado.
            modelBuilder.Entity<PokemonOwner>()
                .HasOne(p => p.Pokemon)
                .WithMany(pc => pc.PokemonOwners)
                .HasForeignKey(p => p.PokemonId); //clave foranea en PokemonOwner que referencia a Pokemon.

            // Cada PokemonOwner tiene un Owner asociado.
            modelBuilder.Entity<PokemonOwner>()
                .HasOne(p => p.Owner)
                .WithMany(pc => pc.PokemonOwners)
                .HasForeignKey(c => c.OwnerId);//clave foranea en PokemonOwner que referencia a Owner.

        }
    }
}
