using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NesneMVC.Models
{
    public class Sepet
    {
        [Key]
        public int SepetID { get; set; }
        public int KullaniciID { get; set; }
        public int UrunID { get; set; }
        public int SepetUrunSayi { get; set; }
        public int SepetToplamFiyat { get; set; }
        public virtual Kullanicilar Kullanicilar { get; set; }
        public virtual Urunler Urunler { get; set; }

    }
}