using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NesneMVC.Models
{
    public class Siparisler
    {
        [Key]
        public int SiparisID { get; set; }
        public int KullaniciID { get; set; }
        public string SiparisAdi { get; set; }
        public int SiparisFiyat { get; set; }
        public int SiparisAdet { get; set; }
        public string SiparisResim { get; set; }
        public string SiparisAdres { get; set; }
        public DateTime SiparisTarih { get; set; }
    }
}