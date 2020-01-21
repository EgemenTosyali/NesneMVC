using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NesneMVC.Models
{
    public class Galeri
    {
        [Key]
        public int GaleriID { get; set; }
        [Required]
        public string GaleriBaslik { get; set; }
        [Required]
        public string GaleriDuyuru { get; set; }
        public string GaleriResim { get; set; }
    }
}