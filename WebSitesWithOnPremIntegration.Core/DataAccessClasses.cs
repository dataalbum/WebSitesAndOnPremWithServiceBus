using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace WebSitesWithOnPremIntegration.Core
{
    public class CustomerDataContext : DbContext
    {
        public CustomerDataContext() : base("CustomerDatabase") { }

        public DbSet<Customer> Customers { get; set; }
    }

    public static class CustomerService
    {
        public static void Add(Customer customer)
        {
            var ctx = new CustomerDataContext();
            ctx.Customers.Add(customer);
            ctx.SaveChanges();
        }
    }

    internal sealed class Configuration : 
        DbMigrationsConfiguration<CustomerDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }
    }
}
