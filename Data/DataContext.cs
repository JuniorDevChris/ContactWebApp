using System;
using ContactAppWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContactAppWeb.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // DbSet property to represent the ContactModel entity in the database
        public DbSet<ContactModel> ContactModels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ContactModel>()
                .Property(c => c.UserId)
                .IsRequired(false);
        }
    }
}
