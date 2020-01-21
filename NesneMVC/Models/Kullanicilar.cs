using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NesneMVC.Models
{
    public class Kullanicilar
    {
        [Key]
        public int KullaniciID { get; set; }
        [Required]
        public string KullaniciKA { get; set; }
        [Required]
        public string KullaniciSF { get; set; }
        [Required]
        public string KullaniciAdi { get; set; }
        [Required]
        public string KullaniciEmail { get; set; }
        [Required]
        public string KullaniciTelefon { get; set; }
        [Required]
        public DateTime KullaniciDogum { get; set; }
        [Required]
        public string KullaniciAdres { get; set; }
        public virtual ICollection<Sepet> Sepets { get; set; }

    }
}