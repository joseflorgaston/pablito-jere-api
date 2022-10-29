using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PablitoJere;
using PablitoJere.DTOs;
using PablitoJere.Entities;
using PablitoJere.Services;

namespace PablitoJere.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly BlobStorageService _blob = new BlobStorageService();
        public PublicationsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Publications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publication>>> GetPublications()
        {
            List<Publication> publications = await _context.Publications
                  .Include(publication => publication.PublicationImages).ToListAsync();

            return publications;
        }

        // GET: api/Publications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Publication>> GetPublication(int id)
        {
          if (_context.Publications == null)
          {
              return NotFound();
          }
            var publication = await _context.Publications.Include(x => x.PublicationImages).Where(x=> x.Id == id).FirstAsync();

            if (publication == null)
            {
                return NotFound();
            }

            return publication;
        }

        // PUT: api/Publications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublication(int id, PublicationCreateDTO publication)
        {
            var oldPublication = await _context.Publications.Include(x => x.PublicationImages).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (oldPublication == null)
            {
                return NotFound();
            }

            oldPublication.Title = publication.Title;
            oldPublication.Description = publication.Description;

            List<string> identifiers = _blob.GetIdentifiers(oldPublication.PublicationImages);
            
            await _blob.DeleteFilesFromContainer(identifiers);
            string[] imageUrls = await _blob.UploadImagesToBlobStorage(publication.PublicationImages);

            oldPublication.PublicationImages = new List<PublicationImage>();

            for (int i = 0; i < publication.PublicationImages.Count; i++)
            {
                PublicationImage image = new PublicationImage();
                image.ImageUrl = imageUrls[i];
                oldPublication.PublicationImages.Add(image);
            }

            _context.Entry(oldPublication).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(publication);
        }

        // POST: api/Publications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult<Publication>> PostPublication(PublicationCreateDTO publicationCreateDTO)
        {
            Publication publication = new Publication();
            publication.Title = publicationCreateDTO.Title;
            publication.Description = publicationCreateDTO.Description;
            publication.PublicationImages = new List<PublicationImage>();
            publication.Date = DateTime.Now;

            string[] imageUrls = await _blob.UploadImagesToBlobStorage(publicationCreateDTO.PublicationImages);

            for(int i = 0; i < publicationCreateDTO.PublicationImages.Count; i++)
            {
                PublicationImage image = new PublicationImage();
                image.ImageUrl = imageUrls[i];
                publication.PublicationImages.Add(image);
            }

            _context.Publications.Add(publication);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPublication", new { id = publication.Id }, publication);
        }

        // DELETE: api/Publications/5
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublication(int id)
        {
            if (_context.Publications == null || await _context.Publications.FindAsync(id) == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications.Include(publication => publication.PublicationImages).Where(x=> x.Id == id).FirstOrDefaultAsync();
            List<string> identifiers = _blob.GetIdentifiers(publication!.PublicationImages);
            _context.Publications.Remove(publication);
            List<Task> tasks = new List<Task>() {_blob.DeleteFilesFromContainer(identifiers), _context.SaveChangesAsync()};
            await Task.WhenAll(tasks);

            return NoContent();
        }


        // DELETE: api/Publications/5
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllPublications()
        {
            if (_context.Publications.Any())
            {
                _context.Publications.RemoveRange(_context.Publications.ToList());
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PublicationExists(int id)
        {
            return (_context.Publications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
