using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndGasApp.Data.Seeder;
using BackEndGasApp.Models.zsk;
using BackEndGasApp.Models.zsk;

namespace BackEndGasApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        //zTables
        public DbSet<zRole> zRoles { get; set; }
        public DbSet<zField> zFields { get; set; }
        public DbSet<zMaintenanceType> zMaintenanceTypes { get; set; }

        //Tables
        public DbSet<ProductionRecord> ProductionRecords { get; set; }
        public DbSet<FieldMaintenance> FieldMaintenances { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed();
        }
    }
}
