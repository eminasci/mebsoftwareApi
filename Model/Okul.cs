    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace mebsoftwareApi.Model
    {
        public class Okul
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int OkulId { get; set; }
           
            public string OkulAdi { get; set; }
            public string? OkulAdres  { get; set;}
            [Required]
            public string OkulTuru { get; set; }

            public string OkulIletisim { get; set; }

          
            public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }


      



    }
    }
