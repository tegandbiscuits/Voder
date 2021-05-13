using System.ComponentModel.DataAnnotations;

namespace Voder.Models
{
    public class Podcast
    {
        [Key]
        public int Id { get; set; }

        [Url]
        [Required]
        public string FeedUrl { get; set; }

        public string Name { get; set; }

        [Url]
        public string CoverUrl { get; set; }
    }
}
