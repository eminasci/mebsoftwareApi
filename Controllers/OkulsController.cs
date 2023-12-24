using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mebsoftwareApi.Model;

namespace mebsoftwareApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OkulsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OkulsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Okuls
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Okul>>> GetOkul()
        {
            return await _context.Okul.ToListAsync();
        }

        // GET: api/Okuls/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Okul>> GetOkul(int id)
        {
            var okul = await _context.Okul.FindAsync(id);

            if (okul == null)
            {
                return NotFound();
            }

            return okul;
        }
        // PUT: api/Okuls/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update-school/{okulId}")]
        public async Task<ActionResult<Okul>> UpdateSchoolUserId(int okulId, [FromBody] OkulUpdateModel okulModel)
        {
            // Okulu bul
            var existingOkul = await _context.Okul.FindAsync(okulId);
            if (existingOkul == null)
            {
                return NotFound("Okul bulunamadı");
            }

            // Kullanıcıyı bul
            var user = await _context.User.FindAsync(okulModel.UserId);
            if (user == null)
            {
                return NotFound("Belirtilen UserId'ye sahip kullanıcı bulunamadı");
            }

            // Okul bilgilerini güncelle
            existingOkul.OkulAdi = okulModel.OkulAdi;
            existingOkul.OkulAdres = okulModel.OkulAdres;
            existingOkul.OkulTuru = okulModel.OkulTuru;
            existingOkul.OkulIletisim = okulModel.OkulIletisim;

            // UserId'yi güncelle
            existingOkul.UserId = okulModel.UserId;

            await _context.SaveChangesAsync();

            return Ok(existingOkul);
        }

        public class OkulUpdateModel
        {
            public string OkulAdi { get; set; }
            public string OkulAdres { get; set; }
            public string OkulTuru { get; set; }
            public string OkulIletisim { get; set; }
            public int UserId { get; set; }
        }

        // POST: api/Okuls
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("add-school/{userId}")]
        public async Task<ActionResult<Okul>> AddUserSchool(int userId, [FromBody] OkulCreateModel okulModel)
        {
            // Kullanıcıyı bul
            var user = await _context.User.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı");
            }

            // Okul bilgilerini ekleyin
            var newOkul = new Okul
            {
                OkulAdi = okulModel.OkulAdi,
                OkulAdres = okulModel.OkulAdres,
                OkulTuru = okulModel.OkulTuru,
                OkulIletisim = okulModel.OkulIletisim,
                UserId = userId
            };

            // Veritabanına kaydedin
            _context.Okul.Add(newOkul);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOkul), new { userId = userId }, newOkul);
        }


        public class OkulCreateModel
        {
            public string OkulAdi { get; set; }
            public string OkulAdres { get; set; }
            public string OkulTuru { get; set; }
            public string OkulIletisim { get; set; }
        }

        // DELETE: api/Okuls/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOkul(int id)
        {
            var okul = await _context.Okul.FindAsync(id);
            if (okul == null)
            {
                return NotFound();
            }

            _context.Okul.Remove(okul);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OkulExists(int id)
        {
            return _context.Okul.Any(e => e.OkulId == id);
        }
    }
}
