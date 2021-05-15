using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Voder.Models;
using Microsoft.Extensions.DependencyInjection;
using AutoFixture;
using System.Linq;

namespace Voder.IntegrationTests.Controllers
{
    public class PodcastsControllerTest : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly VoderContext dbContext;
        private readonly HttpClient client;
        private readonly Fixture fixture;

        public PodcastsControllerTest(TestWebApplicationFactory<Startup> factory)
        {
            var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            this.dbContext = scope.ServiceProvider.GetService<VoderContext>();

            this.client = factory.CreateClient();

            this.fixture = new Fixture();
        }

        [Fact]
        public async Task CanGetPodcasts()
        {
            var testPodcast = this.fixture.Create<Podcast>();
            this.dbContext.Podcasts.Add(testPodcast);
            await this.dbContext.SaveChangesAsync();

            var httpResponse = await this.client.GetAsync("/Podcasts");
            httpResponse.EnsureSuccessStatusCode();

            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var podcasts = JsonConvert.DeserializeObject<IEnumerable<Podcast>>(stringResponse);

            Assert.Single(podcasts);
            Assert.Contains(podcasts, p => p.FeedUrl == testPodcast.FeedUrl);
        }
    }
}
