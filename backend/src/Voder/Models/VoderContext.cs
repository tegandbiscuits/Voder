using Microsoft.EntityFrameworkCore;

namespace Voder.Models
{
    class VoderContext : DbContext
    {
        public VoderContext(DbContextOptions<VoderContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost;Database=VoderDev;Username=teganr");

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseNpgsql("Host=localhost;Database=VoderDev;Username=teganr");
        // }

        public DbSet<Podcast> Podcasts { get; set; }
    }
}
