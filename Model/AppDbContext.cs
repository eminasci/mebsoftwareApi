using Microsoft.EntityFrameworkCore;
using mebsoftwareApi.Model;

namespace mebsoftwareApi.Model
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Okul> Okul { get; set; } = default!;
        public DbSet<User> User { get; set; } = default!;
        public DbSet<Role> Role { get; set; } = default!;
      

        public static void SeedData(AppDbContext context)
        {
            if (!context.Role.Any())
            {
                var roles = new List<Role>
                {
                    new Role
                    {
                        RoleName = "superAdmin"
                    },
                    new Role
                    {
                        RoleName = "admin"
                    },
                  
                };

                context.Role.AddRange(roles);
                context.SaveChanges();
            }
        }
        public DbSet<mebsoftwareApi.Model.Student> Student { get; set; } = default!;



    }
}
