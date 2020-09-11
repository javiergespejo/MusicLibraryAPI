using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public class SongsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SongsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Songs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongDTO>>> GetSong()
        {
            var song = await _context.Song.Select(s =>
            new SongDTO()
            {
                SongId = s.Id,
                SongName = s.Name,
                AlbumName = s.Album.Name,
                ArtistName = s.Album.Artist.Name
            }).ToListAsync();

            return song;
        }

        // GET: api/Songs/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SongDTO>> GetSong(int id)
        {
            var song = await _context.Song.Select(s =>
            new SongDTO()
            {
                SongId = s.Id,
                SongName = s.Name,
                AlbumName = s.Album.Name,
                ArtistName = s.Album.Artist.Name
            }).FirstOrDefaultAsync(s => s.SongId == id);

            if (song == null)
            {
                return NotFound();
            }

            return song;
        }

        //Search by song name
        // GET: api/Songs/name
        [HttpGet("{name}")]
        public async Task<ActionResult<List<Song>>> GetSong(string name)
        {
            var song = await _context.Song.Where(s => s.Name.Contains(name)).ToListAsync();

            if (song == null)
            {
                return NotFound();
            }

            return song;
        }

        // PUT: api/Songs/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSong(int id, Song song)
        {
            if (id != song.Id)
            {
                return BadRequest();
            }

            _context.Entry(song).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(id))
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

        // POST: api/Songs
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<List<Song>>> PostListSong(List<Song> songs)
        {
            var existingSongs = songs.Where(x => _context.Song.Any(z => z.AlbumId == x.AlbumId &&
                                                                            z.Name == x.Name)).ToList();


            if (existingSongs?.Count != 0)
            {
                return BadRequest(existingSongs);
            }
            else
            {
                _context.Song.AddRange(songs);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetSongs", new List<Song>(songs));
            }
        }

        // DELETE: api/Songs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Song>> DeleteSong(int id)
        {
            var song = await _context.Song.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            _context.Song.Remove(song);
            await _context.SaveChangesAsync();

            return song;
        }

        private bool SongExists(int id)
        {
            return _context.Song.Any(e => e.Id == id);
        }
    }
}
