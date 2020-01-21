using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NesneMVC.Models
{
    public class Urunler
    {

        [Key]
        public int UrunID { get; set; }

        public int KategoriID { get; set; }
        [Required]
        public string UrunAdi { get; set; }
        [Required]
        public int UrunFiyat { get; set; }
        [Required]
        public int UrunStok { get; set; }
        [Required]
        public string UrunBilgi { get; set; }  
        public string UrunResim { get; set; }
        public virtual Kategori Kategori { get; set; }
        public virtual ICollection<Sepet> Sepets { get; set; }

    }   
}