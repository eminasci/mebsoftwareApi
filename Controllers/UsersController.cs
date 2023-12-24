using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mebsoftwareApi.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    // Tüm kullanıcıları getir
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _context.User.Include(u => u.Role).ToListAsync();
        return Ok(users);
    }
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLoginModel loginModel)
    {
        // Kullanıcıyı kullanıcı adı ve şifre ile sorgula
        var user = _context.User.SingleOrDefault(u => u.UserEmail == loginModel.UserEmail && u.UserPassword == loginModel.UserPassword);

        if (user == null)
        {
            // Kullanıcı bulunamazsa hata mesajıyla 401 Unauthorized döndür
            return Ok("Kullanıcı adı veya şifre hatalı");
        }

        // Kullanıcı bulundu, başarılı bir giriş mesajıyla 200 OK döndür
        return Ok(user);
    }
    public class UserLoginModel
    {
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
    }

    // Belirli bir kullanıcıyı getir
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("mail/{email}")]
    public async Task<ActionResult<User>> GetUserMail(string email)
    {
        var user = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserEmail == email);

        if (user == null)
        {
            return Ok(404);
        }

        return Ok(200);
    }

    [HttpPost]
    public async Task<ActionResult<User>> AddUser(int roleId, [FromBody] UserCreateModel userModel)
    {
        // RoleId'ye sahip bir Role var mı kontrol et
        var role = await _context.Role.FindAsync(roleId);
        if (role == null)
        {
            return BadRequest("Geçersiz RoleId");
        }

        // Yeni bir kullanıcı oluştur
        var newUser = new User
        {
            UserName = userModel.UserName,
            UserEmail = userModel.UserEmail,
            UserPassword = userModel.UserPassword,
            UserPhoneNumber = userModel.UserPhoneNumber,
            RoleId = roleId
        };

        _context.User.Add(newUser);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = newUser.UserId }, newUser);
    }
    public class UserCreateModel
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserPhoneNumber { get; set; }
    }



    // Belirli bir kullanıcıyı güncelle
    [HttpPut("{id}")]
    public async Task<ActionResult<User>> UpdateUser(int id, int roleId, [FromBody] UserUpdateModel userModel)
    {
        // Kullanıcıyı bul
        var existingUser = await _context.User.FindAsync(id);
        if (existingUser == null)
        {
            return NotFound("Kullanıcı bulunamadı");
        }

        // RoleId'ye sahip bir Role var mı kontrol et
        var role = await _context.Role.FindAsync(roleId);
        if (role == null)
        {
            return BadRequest("Geçersiz RoleId");
        }

        // Kullanıcı bilgilerini güncelle
        existingUser.UserName = userModel.UserName;
        existingUser.UserEmail = userModel.UserEmail;
        existingUser.UserPassword = userModel.UserPassword;
        existingUser.UserPhoneNumber = userModel.UserPhoneNumber;
        existingUser.RoleId = roleId;

        await _context.SaveChangesAsync();

        return Ok(existingUser);
    }
    public class UserUpdateModel
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserPhoneNumber { get; set; }
    }


    // Belirli bir kullanıcıyı sil
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.User.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.User.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Yardımcı metot: Kullanıcı var mı kontrol et
    private bool UserExists(int id)
    {
        return _context.User.Any(e => e.UserId == id);
    }
}