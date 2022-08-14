using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class PublicationImagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PublicationImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PublicationImages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicationImage>>> GetPublicationImages()
        {
          if (_context.PublicationImages == null)
          {
              return NotFound();
          }
            return await _context.PublicationImages.ToListAsync();
        }

        // GET: api/PublicationImages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PublicationImage>> GetPublicationImage(int id)
        {
          if (_context.PublicationImages == null)
          {
              return NotFound();
          }
            var publicationImage = await _context.PublicationImages.FindAsync(id);

            if (publicationImage == null)
            {
                return NotFound();
            }

            return publicationImage;
        }

        // PUT: api/PublicationImages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublicationImage(int id, PublicationImage publicationImage)
        {
            if (id != publicationImage.Id)
            {
                return BadRequest();
            }

            _context.Entry(publicationImage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublicationImageExists(id))
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

        // POST: api/PublicationImages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PublicationImage>> PostPublicationImage(PublicationImageDTO publicationImage)
        {
          if (_context.PublicationImages == null)
          {
              return Problem("Entity set 'ApplicationDbContext.PublicationImages'  is null.");
          }

            BlobStorageService blob = new BlobStorageService();
            publicationImage.ImageSrc = await blob.UploadImageToBlob("Goku", publicationImage.ImageSrc);

            return CreatedAtAction("GetPublicationImage", new { id = publicationImage.Id }, publicationImage);
        }

        // DELETE: api/PublicationImages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublicationImage(int id)
        {
            if (_context.PublicationImages == null)
            {
                return NotFound();
            }
            var publicationImage = await _context.PublicationImages.FindAsync(id);
            if (publicationImage == null)
            {
                return NotFound();
            }

            _context.PublicationImages.Remove(publicationImage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PublicationImages/5
        [HttpDelete("all")]
        public async Task<IActionResult> DeletePublicationImages()
        {
            if (_context.Publications.Any())
            {
                _context.Publications.RemoveRange(_context.Publications.ToList());
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PublicationImageExists(int id)
        {
            return (_context.PublicationImages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
