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
        public DbSet<ErroeProcess> ErroeProcesses { get; set; }

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
            picturePage.HasIndex(m=>m.Url);

            var resource = modelBuilder.Entity<Resource>();
            resource.HasKey(m => m.Id);
            resource.Property(m => m.Id).ValueGeneratedOnAdd();
            resource.HasOne(m => m.PicturePage).WithMany(n => n.Resources).HasForeignKey(m=>m.PicturePageId);
            resource.HasIndex(m=>m.Url);

            var erroeProcess = modelBuilder.Entity<ErroeProcess>();
            erroeProcess.HasKey(m => m.Id);
            erroeProcess.Property(m => m.Id).ValueGeneratedOnAdd();
            erroeProcess.HasIndex(m => m.Url);

            base.OnModelCreating(modelBuilder);
        }
    }
}
