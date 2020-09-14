using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Data;
using MusicLibrary.DTO;
using MusicLibrary.Models;

namespace MusicLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArtistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Artists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArtistDTO>>> GetArtist()
        {
            var artists = await _context.Artist.Select(a =>
            new ArtistDTO()
            {
                ArtistId = a.Id,
                ArtistName = a.Name
            }).ToListAsync();

            return artists;
        }

        // GET: api/Artists/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Artist>> GetArtist(int id)
        {
            var artist = await _context.Artist.FindAsync(id);

            if (artist == null)
            {
                return NotFound();
            }

            return artist;
        }

        //Search by artist name
        // GET: api/Artists/name
        [HttpGet("{name}")]
        public async Task<ActionResult<List<ArtistDTO>>> GetArtist(string name)
        {
            var artists = await _context.Artist.Where(a=>a.Name.Contains(name)).Select(a =>
            new ArtistDTO()
            {
                ArtistId = a.Id,
                ArtistName = a.Name
            }).ToListAsync();

            if (artists == null)
            {
                return NotFound();
            }

            return artists;
        }

        // PUT: api/Artists/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArtist(int id, Artist artist)
        {
            if (id != artist.Id)
            {
                return BadRequest();
            }

            _context.Entry(artist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtistExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Artists
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<List<Artist>>> PostListArtist(List<Artist> artists)
        {
            var existingArtists = artists.Where(a => _context.Artist.Any(x => x.Id == a.Id &&
                                                                         x.Name == a.Name)).ToList();

            if (existingArtists?.Count != 0)
            {
                return BadRequest(existingArtists);
            }
            else
            {
                _context.Artist.AddRange(artists);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetArtist", new List<Artist>(artists));
            }
        }

        // DELETE: api/Artists/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Artist>> DeleteArtist(int id)
        {
            var artist = await _context.Artist.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }

            _context.Artist.Remove(artist);
            await _context.SaveChangesAsync();

            return artist;
        }

        private bool ArtistExists(int id)
        {
            return _context.Artist.Any(e => e.Id == id);
        }
    }
}
