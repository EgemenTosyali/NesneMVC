using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NesneMVC.Models
{
    public class Kategori
    {

        [Key]
        public int KategoriID { get; set; }
        [Required]
        public string KategoriAdi { get; set; }
        public virtual ICollection<Urunler> Urunlers { get; set; }
    }
}