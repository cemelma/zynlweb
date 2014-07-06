using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Lütfen Adınızı Giriniz")]
        [Display(Name = "Ad-Soyad")]
        public string FullName { get; set; }

        [Display(Name = "Şirket")]
        public string Company { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Telefon")]
        public string Tel { get; set; }

        [Display(Name = "Cep")]
        public string Cep { get; set; }

        [Display(Name = "Fax")]
        public string Fax { get; set; }

        [Required(ErrorMessage = "Lütfen Emailinizi Giriniz")]
        [Display(Name = "E-Posta Adresi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lütfen Şifrenizi Giriniz")]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Display(Name = "Eklenme Tarihi")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime TimeUpdated { get; set; }

        public bool Online { get; set; }

        public bool Deleted { get; set; }

    }
}
