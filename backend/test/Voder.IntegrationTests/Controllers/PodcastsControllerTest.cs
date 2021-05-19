using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Voder.Models;
using AutoFixture;
using System.Linq;
using System.Net;

namespace Voder.IntegrationTests.Controllers
{
    public class PodcastsControllerTest : AbstractControllerTest
    {
        public PodcastsControllerTest(TestWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        [Trait("PodcastsController", "Index")]
        public async Task GetsThePodcasts()
        {
            var testPodcast = this.fixture.Create<Podcast>();
            this.dbContext.Podcasts.Add(testPodcast);
            this.dbContext.Podcasts.Add(this.fixture.Create<Podcast>());
            await this.dbContext.SaveChangesAsync();

            var podcasts = await this.SendRequest<IEnumerable<Podcast>>("/Podcasts");

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

            var returnedPodcast = await this.SendRequest<Podcast>($"/Podcasts/{testPodcast.Id}");

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
            var createdPodcast = await this.SendRequest<Podcast>("/Podcasts", method: HttpMethod.Post, body: body);

            Assert.Equal(podcastCount + 1, this.dbContext.Podcasts.Count());
            Assert.True(createdPodcast.Id > 0);
        }

        [Fact]
        [Trait("PodcastsController", "Removal")]
        public async Task DeletesThePodcast()
        {
            var testPodcast = this.fixture.Create<Podcast>();
            this.dbContext.Podcasts.Add(testPodcast);
            await this.dbContext.SaveChangesAsync();

            await this.SendRequest<object>($"/Podcasts/{testPodcast.Id}", method: HttpMethod.Delete);

            var podcasts = await this.SendRequest<IEnumerable<Podcast>>("/Podcasts");
            Assert.DoesNotContain(podcasts, p => p.Id == testPodcast.Id);
        }
    }
}
