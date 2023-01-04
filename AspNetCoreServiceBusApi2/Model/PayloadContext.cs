using Microsoft.EntityFrameworkCore;

namespace ServiceBusReceiverApi.Model
{
    public class PayloadContext : DbContext
    {
        public PayloadContext(DbContextOptions<PayloadContext> options) : base(options)
        {
        }

        public DbSet<Payload> Payloads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payload>().Property(n => n.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Payload>().HasKey(m => m.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}