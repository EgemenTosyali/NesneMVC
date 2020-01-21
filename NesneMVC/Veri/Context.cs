using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using NesneMVC.Models;

namespace NesneMVC.Veri
{
    public class Context:DbContext
    {
        public Context() : base() {}
        public DbSet<Urunler> Urunlers { get; set; }
        public DbSet<Kategori> Kategoris { get; set; }
        public DbSet<Kullanicilar> Kullanicilars { get; set; }
        public DbSet<Yoneticiler> Yoneticilers { get; set; }
        public DbSet<Sepet> Sepets { get; set; }
        public DbSet<Galeri> Galeris { get; set; }
        public DbSet<Siparisler> Siparislers { get; set; }
        public object KategoriId { get; internal set; }
    }
}