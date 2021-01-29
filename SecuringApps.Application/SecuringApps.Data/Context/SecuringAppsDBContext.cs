using Microsoft.EntityFrameworkCore;
using SecuringApps.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecuringApps.Data.Context
{
 
    public class SecuringAppsDBContext : DbContext
    {
        public SecuringAppsDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           //   modelBuilder.Entity<Member>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            //modelBuilder.Entity<Category>().Property(x => x.Id).HasDefaultValueSql("NEWID()");

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }


    }
}
