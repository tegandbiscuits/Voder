using Voder.Models;

namespace Voder.IntegrationTests
{
    public class SeedData
    {
        public static void PopulateTestData(VoderContext dbContext)
        {
            var firstPodcast = new Podcast
            {
                FeedUrl = "http://example.com",
            };

            dbContext.Podcasts.Add(firstPodcast);
            dbContext.SaveChanges();
        }
    }
}
