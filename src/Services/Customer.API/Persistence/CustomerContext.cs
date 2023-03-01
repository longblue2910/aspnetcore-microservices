using Microsoft.EntityFrameworkCore;

namespace Customer.API.Persistence
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        {

        }

        public DbSet<Customer.API.Entities.Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Customer.API.Entities.Customer>().HasIndex(x => x.UserName).IsUnique();
            modelBuilder.Entity<Customer.API.Entities.Customer>().HasIndex(x => x.EmailAddress).IsUnique();

        }
    }
}
