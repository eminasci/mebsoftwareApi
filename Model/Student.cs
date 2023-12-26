using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mebsoftwareApi.Model
{
    public class Student
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OgrenciId { get; set; }
        [Required]
        public string OgrenciName { get; set; }
        [Required]
        public string OgrenciTc { get; set; }

        public int OgrenciDevamsizlik { get; set; }
        [Required]
        public string OgrenciSinif { get; set; }
        
        public string ?OgrenciDurum {  get; set; }
        public string OgrenciPhoneNumber { get; set; }

        

        public string VeliName {  get; set; }
        public string VeliPhoneNumber { get; set; }



        [Required]
        public int OkulId { get; set; }

        [ForeignKey(nameof(OkulId))]
        public virtual Okul Okul { get; set; }






    }
}


