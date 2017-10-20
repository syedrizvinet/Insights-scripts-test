﻿using Microsoft.EntityFrameworkCore;

namespace Knapcode.ExplorePackages.Entities
{
    public class EntityContext : DbContext
    {
        public DbSet<Package> Packages { get; set; }
        public DbSet<Cursor> Cursors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite("Data Source=ExplorePackages.sqlite3");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Cursor>()
                .HasKey(x => x.Name);

            modelBuilder
                .Entity<Cursor>()
                .Property(x => x.RowVersion)
                .IsRowVersion();

            modelBuilder
                .Entity<Package>()
                .Property(x => x.Key)
                .HasColumnName("PackageKey");

            modelBuilder
                .Entity<Package>()
                .HasKey(x => x.Key);

            modelBuilder
                .Entity<Package>()
                .Property(x => x.Id)
                .HasColumnType("TEXT COLLATE NOCASE")
                .IsRequired();

            modelBuilder
                .Entity<Package>()
                .Property(x => x.Version)
                .HasColumnType("TEXT COLLATE NOCASE")
                .IsRequired();

            modelBuilder
                .Entity<Package>()
                .Property(x => x.Identity)
                .HasColumnType("TEXT COLLATE NOCASE")
                .IsRequired();

            modelBuilder
                .Entity<Package>()
                .HasIndex(x => new { x.Identity })
                .IsUnique();

            modelBuilder
                .Entity<Package>()
                .HasIndex(x => new { x.Id, x.Version })
                .IsUnique();

            modelBuilder
                .Entity<Package>()
                .Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}
