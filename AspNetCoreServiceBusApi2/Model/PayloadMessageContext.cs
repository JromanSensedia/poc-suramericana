using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ServiceBusReceiverApi.Model
{
    public class PayloadMessageContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Payload> Payloads { get; set; }

        public PayloadMessageContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payload>().Property(n => n.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Payload>().HasKey(m => m.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}