using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Voder.Models;
using Microsoft.AspNetCore.Http;

namespace Voder.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PodcastsController : ControllerBase
    {
        private readonly VoderContext _context;

        public PodcastsController(VoderContext voderContext)
        {
            _context = voderContext;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<Podcast>> GetPodcasts()
        {
            return await Task.Run<IEnumerable<Podcast>>(() => _context.Podcasts.ToList<Podcast>());
        }

        [HttpGet("{podcastId}")]
        public async Task<ActionResult<Podcast>> GetPodcast([FromRoute] int podcastId)
        {
            var podcast = await _context.Podcasts.FindAsync(podcastId);

            if (podcast == null)
            {
                return NotFound();
            }

            return Ok(podcast);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Podcast>> PostPodcast([FromBody] Podcast podcast)
        {
            _context.Podcasts.Add(podcast);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPodcast), new { podcastId = podcast.Id }, podcast);
        }

        [HttpDelete("{podcastId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeletePodcast([FromRoute] int podcastId)
        {
            var podcast = await _context.Podcasts.FindAsync(podcastId);
            _context.Podcasts.Remove(podcast);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
