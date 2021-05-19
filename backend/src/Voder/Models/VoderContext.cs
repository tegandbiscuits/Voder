using Microsoft.EntityFrameworkCore;

namespace Voder.Models
{
    public class VoderContext : DbContext
    {
        public VoderContext(DbContextOptions<VoderContext> options)
            : base(options)
        {
        }

        public DbSet<Podcast> Podcasts { get; set; }
    }
}
