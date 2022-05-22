using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data
{
    public class Context : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Blade> Blades { get; set; }
        public DbSet<Brand> Brands { get; set; }

        public Context(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Player>().ToTable("Player");
            builder.Entity<Player>().HasKey(p => p.Id);
            builder.Entity<Player>().Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Entity<Player>().HasOne(p => p.Country).WithMany();

            builder.Entity<Country>().ToTable("Country");
            builder.Entity<Country>().HasKey(c => c.Id);
            builder.Entity<Country>().Property(c => c.Id).ValueGeneratedOnAdd();

            builder.Entity<Blade>().ToTable("Blade");
            builder.Entity<Blade>().HasKey(b => b.Id);
            builder.Entity<Blade>().Property(b => b.Id).ValueGeneratedOnAdd();
            builder.Entity<Blade>().HasOne(b => b.Brand).WithMany();

            builder.Entity<Brand>().ToTable("Brand");
            builder.Entity<Brand>().HasKey(b => b.Id);
            builder.Entity<Brand>().Property(b => b.Id).ValueGeneratedOnAdd();
        }
    }
}
