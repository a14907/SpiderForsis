using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderForSis001.Data
{
    public class MyDbContext : DbContext
    {
        public DbSet<MoviePage> MoviePages { get; set; }
        public DbSet<Resource> Resources { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SqliteConnectionStringBuilder sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder() {
                DataSource= "MyDb.db"
            };
            optionsBuilder.UseSqlite(new SqliteConnection(sqliteConnectionStringBuilder.ToString()));
             

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var picturePage = modelBuilder.Entity<MoviePage>();
            picturePage.HasKey(m => m.Id);
            picturePage.Property(m => m.Id).ValueGeneratedOnAdd();
            picturePage.HasIndex(m=>m.Name);

            var resource = modelBuilder.Entity<Resource>();
            resource.HasKey(m => m.Id);
            resource.Property(m => m.Id).ValueGeneratedOnAdd();
            resource.HasOne(m => m.PicturePage).WithMany(n => n.Resources).HasForeignKey(m=>m.PicturePageId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
