using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Voder.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Voder.IntegrationTests.Controllers
{
    public class PodcastsControllerTest : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly TestWebApplicationFactory<Startup> factory;
        private readonly HttpClient client;

        public PodcastsControllerTest(TestWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            this.client = factory.CreateClient();
        }

        [Fact]
        public async Task CanGetPodcasts()
        {
            var scopeFactory = this.factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<VoderContext>();

            var p = new Podcast
            {
                FeedUrl = "http://example.com",
            };
            context.Podcasts.Add(p);
            await context.SaveChangesAsync();

            var httpResponse = await this.client.GetAsync("/Podcasts");
            httpResponse.EnsureSuccessStatusCode();

            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var podcasts = JsonConvert.DeserializeObject<IEnumerable<Podcast>>(stringResponse);

            Assert.Contains(podcasts, p => p.FeedUrl == "http://example.com");
        }
    }
}
