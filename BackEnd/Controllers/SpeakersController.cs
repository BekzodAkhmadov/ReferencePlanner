﻿using BackEnd.Data;
using ConferenceDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeakersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpeakersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Speakers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpeakerResponse>>> GetSpeakers()
        {
            var speakers = await _context.Speakers.AsNoTracking()
                 .Include(s => s.SessionSpeakers)
                 .ThenInclude(ss => ss.Session)
                 .Select(s => s.MapSpeakerResponse())
                 .ToListAsync();
            return speakers;
        }

        // GET: api/Speakers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SpeakerResponse>> GetSpeaker(int id)
        {
            var speaker = await _context.Speakers.AsNoTracking()
                .Include(s => s.SessionSpeakers)
                .ThenInclude(ss => ss.Session)
                .SingleOrDefaultAsync(s => s.Id == id);

            if (speaker == null)
            {
                return NotFound();
            }

            return speaker.MapSpeakerResponse();
        }
    }
}
