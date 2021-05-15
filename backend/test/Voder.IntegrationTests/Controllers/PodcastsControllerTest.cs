using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Voder.Models;
using Microsoft.Extensions.DependencyInjection;
using AutoFixture;
using System.Linq;
using System.Net;
using System.Text;

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
        [Trait("PodcastsController", "Index")]
        public async Task GetsThePodcasts()
        {
            var testPodcast = this.fixture.Create<Podcast>();
            this.dbContext.Podcasts.Add(testPodcast);
            this.dbContext.Podcasts.Add(this.fixture.Create<Podcast>());
            await this.dbContext.SaveChangesAsync();

            var httpResponse = await this.client.GetAsync("/Podcasts");
            httpResponse.EnsureSuccessStatusCode();

            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var podcasts = JsonConvert.DeserializeObject<IEnumerable<Podcast>>(stringResponse);

            Assert.Equal(2, podcasts.Count());
            Assert.Contains(podcasts, p => p.FeedUrl == testPodcast.FeedUrl);
        }

        [Fact]
        [Trait("PodcastsController", "Show")]
        public async Task ReturnsThePodcastWhenItExists()
        {
            var testPodcast = this.fixture.Create<Podcast>();
            this.dbContext.Podcasts.Add(testPodcast);
            await this.dbContext.SaveChangesAsync();

            var httpResponse = await this.client.GetAsync($"/Podcasts/{testPodcast.Id}");
            httpResponse.EnsureSuccessStatusCode();

            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var returnedPodcast = JsonConvert.DeserializeObject<Podcast>(stringResponse);

            Assert.Equal(testPodcast.Id, returnedPodcast.Id);
        }

        [Fact]
        [Trait("PodcastsController", "Show")]
        public async Task ReturnsAnErrorWhenThePodcastDoesNotExist()
        {
            var httpResponse = await this.client.GetAsync("/Podcasts/123");
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        [Fact]
        [Trait("PodcastsController", "Create")]
        public async Task CreatesAPodcast()
        {
            var podcastCount = this.dbContext.Podcasts.Count();

            var body = new { FeedUrl = "http://test.com" };
            var httpContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var httpResponse = await this.client.PostAsync("/Podcasts", httpContent);
            httpResponse.EnsureSuccessStatusCode();

            Assert.Equal(podcastCount + 1, this.dbContext.Podcasts.Count());
        }

        [Fact]
        [Trait("PodcastsController", "Removal")]
        public async Task DeletesThePodcast()
        {
            var testPodcast = this.fixture.Create<Podcast>();
            this.dbContext.Podcasts.Add(testPodcast);
            await this.dbContext.SaveChangesAsync();

            var deleteResposne = await this.client.DeleteAsync($"/Podcasts/{testPodcast.Id}");
            deleteResposne.EnsureSuccessStatusCode();

            var indexResponse = await this.client.GetAsync("/Podcasts");
            indexResponse.EnsureSuccessStatusCode();
            var stringResponse = await indexResponse.Content.ReadAsStringAsync();
            var podcasts = JsonConvert.DeserializeObject<IEnumerable<Podcast>>(stringResponse);
            Assert.DoesNotContain(podcasts, p => p.Id == testPodcast.Id);
        }
    }
}
