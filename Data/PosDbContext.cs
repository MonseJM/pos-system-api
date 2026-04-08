using Microsoft.EntityFrameworkCore;
using Mi_punto_de_venta.Entities;


namespace Mi_punto_de_venta.Data

{
    public class PosDbContext:DbContext
    {

        public PosDbContext(DbContextOptions<PosDbContext>options)
            : base(options) { }

        /*usamos aqui las tablas de la base de datos PosDb*/
        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<SaleDetail> SalesDetails { get; set; }
        
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.UserId);
        }
    }
}
