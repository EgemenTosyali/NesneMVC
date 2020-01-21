using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NesneMVC.Models
{
    public class Yoneticiler
    {
        [Key]
        public int YoneticiID { get; set; }
        [Required]
        public string YoneticiKA { get; set; }
        [Required]
        public string YoneticiSF { get; set; }
        [Required]
        public string YoneticiAdi { get; set; }
        [Required]
        public string YoneticiEmail { get; set; }
        [Required]
        public string YoneticiTelefon { get; set; }
        [Required]
        public DateTime YoneticiDogum { get; set; }
        [Required]
        public string YoneticiAdres { get; set; }
    }

}