using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndGasApp.Services.DatabaseService
{
    public class DatabaseService : IDatabaseService
    {
        private readonly DataContext _context;
        public DatabaseService(DataContext context)
        {
            _context = context;

        }
        public void add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}