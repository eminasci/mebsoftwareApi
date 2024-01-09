using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mebsoftwareApi.Model;
using System.IO;
using OfficeOpenXml;

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
        [HttpPost("add-student-admin-from-excel")]
        public async Task<ActionResult<List<Student>>> AddAdminStudentsFromExcel([FromBody] byte[] excelFile)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(excelFile))
                using (ExcelPackage package = new ExcelPackage(memoryStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is in the first sheet

                    List<Student> newStudents = new List<Student>();

                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var studentModel = new AdminStudentCreateModel
                        {
                            OgrenciName = worksheet.Cells[row, 1].Value?.ToString(),
                            OgrenciTc = worksheet.Cells[row, 2].Value?.ToString(),
                            OgrenciDevamsizlik = Convert.ToInt32(worksheet.Cells[row, 3].Value),
                            OgrenciSinif = worksheet.Cells[row, 4].Value?.ToString(),
                            OgrenciPhoneNumber = worksheet.Cells[row, 5].Value?.ToString(),
                            OgrenciDurum = worksheet.Cells[row, 6].Value?.ToString(),
                            VeliName = worksheet.Cells[row, 7].Value?.ToString(),
                            VeliPhoneNumber = worksheet.Cells[row, 8].Value?.ToString(),
                            
                        };

                        // Kullanıcının bağlı olduğu okulu bul
                        var okul = _context.Okul.FirstOrDefault(o => o.UserId == studentModel.UserId);

                        // Eğer okul bulunamazsa
                        if (okul == null)
                        {
                            return BadRequest($"Kullanıcının bağlı olduğu okul bulunamadı. Satır: {row}");
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
                            OkulId = okul.OkulId,
                            VeliName = studentModel.VeliName,
                            VeliPhoneNumber = studentModel.VeliPhoneNumber,
                        };

                        // Veritabanına kaydedin
                        _context.Student.Add(newStudent);
                        await _context.SaveChangesAsync();

                        newStudents.Add(newStudent);
                    }

                    return CreatedAtAction(nameof(GetStudent), newStudents);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }





        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudent()
        {
            var students = await _context.Student.Include(s => s.Okul).ToListAsync();
            return students;
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
       
        [HttpGet("list-students/{okulId}")]
        public ActionResult<List<Student>> ListStudentsByOkulId(int okulId)
        {
            var students = _context.Student
                .Where(s => s.OkulId == okulId)
                .ToList();

            if (students == null || students.Count == 0)
            {
                return NotFound("Okula bağlı öğrenci bulunamadı");
            }

            return Ok(students);
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
            existingStudent.VeliName = studentModel.VeliName;
            existingStudent.VeliPhoneNumber = studentModel.VeliPhoneNumber;

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
            
            public string VeliName { get; set; }

            public string VeliPhoneNumber { get; set; }


            public int OkulId { get; set; }
        }
        [HttpPost("add-student-admin")]
        public async Task<ActionResult<Student>> AddStudent([FromBody] AdminStudentCreateModel studentModel)
        {
            try
            {
                // Kullanıcının bağlı olduğu okulu bul
                var okul = _context.Okul.FirstOrDefault(o => o.UserId == studentModel.UserId);

                // Eğer okul bulunamazsa
                if (okul == null)
                {
                    return BadRequest("Kullanıcının bağlı olduğu okul bulunamadı");
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
                    OkulId = okul.OkulId,  // Kullanıcının bağlı olduğu okulun Id'sini kullan
                    VeliName = studentModel.VeliName,
                    VeliPhoneNumber = studentModel.VeliPhoneNumber,
                };

                // Veritabanına kaydedin
                _context.Student.Add(newStudent);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetStudent), new { studentId = newStudent.OgrenciId }, newStudent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        public class AdminStudentCreateModel
        {
            public string OgrenciName { get; set; }
            public string OgrenciTc { get; set; }
            public int OgrenciDevamsizlik { get; set; }
            public string OgrenciDurum { get; set; }
            public string OgrenciSinif { get; set; }
            public string OgrenciPhoneNumber { get; set; }
            public string VeliName { get; set; }
            public string VeliPhoneNumber { get; set; }
            public int UserId { get; set; }  // Eklenen öğrencinin bağlı olduğu kullanıcının Id'si
        }


        [HttpPost("add-student-from-excel")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<List<Student>>> AddStudentsFromExcel([FromForm] IFormFile excelFile)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(memoryStream);
                    using (ExcelPackage package = new ExcelPackage(memoryStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is in the first sheet

                        List<StudentCreateModel> newStudents = new List<StudentCreateModel>();

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            var studentModel = new StudentCreateModel
                            {
                                OgrenciName = worksheet.Cells[row, 1].Value?.ToString(),
                                OgrenciTc = worksheet.Cells[row, 2].Value?.ToString(),
                                OgrenciDevamsizlik = Convert.ToInt32(worksheet.Cells[row, 3].Value),
                                OgrenciSinif = worksheet.Cells[row, 4].Value?.ToString(),
                                OgrenciPhoneNumber = worksheet.Cells[row, 5].Value?.ToString(),
                                OgrenciDurum = worksheet.Cells[row, 6].Value?.ToString(),
                                VeliName = worksheet.Cells[row, 7].Value?.ToString(),
                                VeliPhoneNumber = worksheet.Cells[row, 8].Value?.ToString(),
                                OkulId = Convert.ToInt32(worksheet.Cells[row, 9].Value), // OkulId sütunu
                            };

                            // Öğrenciyi ekleyin
                            var result = await AddStudent(studentModel);

                            if (result.Result is CreatedAtActionResult)
                            {
                                // Başarıyla eklenen öğrenciyi listeye ekleyin
                                newStudents.Add(studentModel);
                            }
                        }

                        return CreatedAtAction(nameof(GetStudent), newStudents);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
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
                OkulId = studentModel.OkulId,
                VeliName = studentModel.VeliName,
                VeliPhoneNumber = studentModel.VeliPhoneNumber,
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

            public string VeliName { get; set; }
            
            public  string VeliPhoneNumber { get; set; }


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
