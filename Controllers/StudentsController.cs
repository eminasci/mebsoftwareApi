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
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudent()
        {
            return await _context.Student.ToListAsync();
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Student.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update-student/{studentId}")]
        public async Task<ActionResult<Student>> UpdateStudent(int studentId, [FromBody] StudentUpdateModel studentModel)
        {
            // Öğrenciyi bul
            var existingStudent = await _context.Student.FindAsync(studentId);
            if (existingStudent == null)
            {
                return NotFound("Öğrenci bulunamadı");
            }

            // Okulu bul
            var okul = await _context.Okul.FindAsync(studentModel.OkulId);

            // Eğer okul bulunamazsa
            if (okul == null)
            {
                return BadRequest("Belirtilen OkulId'ye sahip okul bulunamadı");
            }

            // Öğrenci bilgilerini güncelle
            existingStudent.OgrenciName = studentModel.OgrenciName;
            existingStudent.OgrenciTc = studentModel.OgrenciTc;
            existingStudent.OgrenciDevamsizlik = studentModel.OgrenciDevamsizlik;
            existingStudent.OgrenciSinif = studentModel.OgrenciSinif;
            existingStudent.OgrenciPhoneNumber = studentModel.OgrenciPhoneNumber;
            existingStudent.OgrenciDurum = studentModel.OgrenciDurum;
            existingStudent.OkulId = studentModel.OkulId;

            // Veritabanına kaydedin
            await _context.SaveChangesAsync();

            return Ok(existingStudent);
        }

        public class StudentUpdateModel
        {
            public string OgrenciName { get; set; }


            public string OgrenciTc { get; set; }

            public int OgrenciDevamsizlik { get; set; }


            public string OgrenciSinif { get; set; }

            public string OgrenciDurum { get; set; }

            public string OgrenciPhoneNumber { get; set; }


            public int OkulId { get; set; }
        }

        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("add-student")]
        public async Task<ActionResult<Student>> AddStudent([FromBody] StudentCreateModel studentModel)
        {
            // Okulu bul
            var okul = await _context.Okul.FindAsync(studentModel.OkulId);

            // Eğer okul bulunamazsa
            if (okul == null)
            {
                return BadRequest("Belirtilen OkulId'ye sahip okul bulunamadı");
            }

            // Öğrenci bilgilerini ekleyin
            var newStudent = new Student
            {
                OgrenciName = studentModel.OgrenciName,
                OgrenciTc = studentModel.OgrenciTc,
                OgrenciDevamsizlik = studentModel.OgrenciDevamsizlik,
                OgrenciSinif = studentModel.OgrenciSinif,
                OgrenciPhoneNumber = studentModel.OgrenciPhoneNumber,
                OgrenciDurum = studentModel.OgrenciDurum,
                OkulId = studentModel.OkulId
            };

            // Veritabanına kaydedin
            _context.Student.Add(newStudent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { studentId = newStudent.OgrenciId }, newStudent);
        }

        public class StudentCreateModel
        {

            public string OgrenciName { get; set; }


            public string OgrenciTc { get; set; }

            public int OgrenciDevamsizlik { get; set; }

            public string OgrenciDurum { get; set; }


            public string OgrenciSinif { get; set; }

            public string OgrenciPhoneNumber { get; set; }


            public int OkulId { get; set; }
        }


        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Student.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.OgrenciId == id);
        }
    }
}
