using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voder.Models;

namespace Voder.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PodcastsController : ControllerBase
    {
        private readonly VoderContext context;

        public PodcastsController(VoderContext voderContext)
        {
            this.context = voderContext;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<Podcast>> GetPodcasts()
        {
            return await Task.Run<IEnumerable<Podcast>>(() => this.context.Podcasts.ToList<Podcast>());
        }

        [HttpGet("{podcastId}")]
        public async Task<ActionResult<Podcast>> GetPodcast([FromRoute] int podcastId)
        {
            var podcast = await this.context.Podcasts.FindAsync(podcastId);

            if (podcast == null)
            {
                return this.NotFound();
            }

            return this.Ok(podcast);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Podcast>> PostPodcast([FromBody] Podcast podcast)
        {
            this.context.Podcasts.Add(podcast);
            await this.context.SaveChangesAsync();

            return this.CreatedAtAction(nameof(this.GetPodcast), new { podcastId = podcast.Id }, podcast);
        }

        [HttpDelete("{podcastId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeletePodcast([FromRoute] int podcastId)
        {
            var podcast = await this.context.Podcasts.FindAsync(podcastId);
            this.context.Podcasts.Remove(podcast);
            await this.context.SaveChangesAsync();
            return this.NoContent();
        }
    }
}
