using Entities;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System;

namespace CMService.Models
{
    public static class DbContextExtensions
    {
        public static void LogToConsole(this DbContext context)
        {
            var service = ((IAccessor<IServiceProvider>)context).Service;
            var loggerFactory = service.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddConsole(LogLevel.Verbose);
        }
    }

    public class CustomerDbContext : DbContext
    {
        private readonly string _connectionString;

        public CustomerDbContext()
        {
        }

        public CustomerDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerUpdate> CustomerUpdates { get; set; }


        protected override void OnConfiguring(EntityOptionsBuilder options)
        {
            if (!string.IsNullOrEmpty(_connectionString))
                options.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Customer>(c =>
            //{
            //    c.Property(e => e.Id).ForSqlServer().UseIdentity();
            //});

            //modelBuilder.Entity<CustomerUpdate>(c =>
            //{
            //    c.Property(e => e.Id).ForSqlServer().UseIdentity();
            //});

            //modelBuilder.Entity<CustomerUpdate>(b => b.Reference("Customer")
            //        .InverseCollection()
            //        .ForeignKey("CustomerId"));


            //var customer = modelBuilder.Entity<Customer>().Table("Customer");
            //customer.Key(c => c.Id);
            //customer.Property(value => value.Name).Required();
            //customer.Property(value => value.Gender).Required();
            //customer.Property(value => value.HouseNumber).Required();
            //customer.Property(value => value.AddressLine1).Required();
            //customer.Property(value => value.State).Required();
            //customer.Property(value => value.Country).Required();
            //customer.Property(value => value.Category).Required();
            //customer.Property(value => value.DateOfBirth).Required();
            //customer.Collection(c => c.Updates).InverseReference();

            //var customerUpdate = modelBuilder.Entity<CustomerUpdate>().Table("CustomerUpdate");
            //customerUpdate.Key(c => c.Id);
            //customerUpdate.Property(c => c.Type);
            //customerUpdate.Property(c => c.Timestamp);
            //customerUpdate.Reference(c => c.Customer);
        }
    }
}
