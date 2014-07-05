using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

          [Display(Name = "Ürün Grubu")]
        [Required(ErrorMessage = "Ürün Grubunu Seçiniz.")]
        public int ProductGroupId { get; set; }
      

        [Display(Name = "Ürün Adı")]
        [Required(ErrorMessage="Ürün Adını Giriniz.")]
        public string Name { get; set; }

      

        [Display(Name = "İçerik")]
        [Required(ErrorMessage = "İçerik Giriniz.")]
        public string Content { get; set; }

        //[Display(Name = "Galeri GrupId")]
        //[Required(ErrorMessage = "Galeri Seçiniz.")]
        //public string GalleryId { get; set; }
        //public Gallery Gallery { get; set; }


       // [Display(Name = "Ürün Fiyatı")]
       // [Required(ErrorMessage = "Ürün Fiyatını Giriniz.")]
       //// [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.00}")]
       // public decimal Price { get; set; }

        //[Display(Name = "Ürün Donanım Fiyatı")]
       
        //public decimal ? HardwarePrice { get; set; }

        //[Display(Name = "Ürün Donanımı Varmı?")]
        //public bool Hardware { get; set; }

        

       

       
        public bool Deleted { get; set; }
        public bool Online { get; set; }
        public DateTime TimeCreated { get; set; }
        public int SortNumber { get; set; }
        [Display(Name = "Ürün Açıklaması")]
        public string PageSlug { get; set; }

      
        
    }
}
