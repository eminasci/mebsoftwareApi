using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mebsoftwareApi.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string UserPassword { get; set; }
      
       public string? UserPhoneNumber { get; set; }


        public int RoleId { get; set; }
        
       
        


        // Kullanıcıya ait rol
        [ForeignKey(nameof(RoleId))]
        public virtual  Role Role { get; set; }







    }
}
