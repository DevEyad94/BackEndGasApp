using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndGasApp.Models.zsk;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BackEndGasApp.Data.Seeder
{
    public static class ModelBuilderExtensions
    {
        private static DataContext _context;

        public static void Seed(this ModelBuilder modelBuilder)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver(),
            };

            modelBuilder.Entity<zField>().HasData(SeedData<zField>(@"Data/Seeder/zsk/zField.json"));
            modelBuilder
                .Entity<zMaintenanceType>()
                .HasData(SeedData<zMaintenanceType>(@"Data/Seeder/zsk/zMaintenanceType.json"));
            modelBuilder.Entity<zRole>().HasData(SeedData<zRole>(@"Data/Seeder/zsk/zRole.json"));
        }

        public static List<T> SeedData<T>(string path)
        {
            var data = new List<T>();
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<T>>(json);
            }
            return data;
        }
    }
}
