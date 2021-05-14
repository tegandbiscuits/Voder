using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Voder.Models;

namespace Voder.IntegrationTests.Controllers
{
    public class PodcastsControllerTest : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;

        public PodcastsControllerTest(TestWebApplicationFactory<Startup> factory)
        {
            this.client = factory.CreateClient();
        }

        [Fact]
        public async Task CanGetPodcasts()
        {
            var httpResponse = await this.client.GetAsync("/Podcasts");
            httpResponse.EnsureSuccessStatusCode();

            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var podcasts = JsonConvert.DeserializeObject<IEnumerable<Podcast>>(stringResponse);

            Assert.Contains(podcasts, p => p.FeedUrl == "http://example.com");
        }
    }
}
