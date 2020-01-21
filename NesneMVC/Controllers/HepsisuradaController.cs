using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NesneMVC.Models;
using NesneMVC.Veri;

namespace NesneMVC.Controllers
{
    public class HepsisuradaController : Controller
    {
        private Context db = new Context();
        public ActionResult Index(int id=0,string arama=null)
        {
            List<Kategori> Kategoris = db.Kategoris.ToList();
            ViewBag.kategori = Kategoris;

            List<Galeri> Galeris = db.Galeris.ToList();
            ViewBag.galeri = Galeris;

            ViewBag.ilkmi = 1;

            var urunler = from u in db.Urunlers select u;
            if (!string.IsNullOrEmpty(arama))
            {
                urunler = db.Urunlers.Where(u => u.UrunAdi.Contains(arama));
            }
            else if (id == 0)
            {
                urunler = from u in db.Urunlers select u;
            }
            else
            {
                urunler = from u in db.Urunlers where u.KategoriID==id select u;
            }
            return View(urunler);
        }
        public ActionResult Giris()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Giris(Kullanicilar k)
        {
            var dene = db.Kullanicilars.FirstOrDefault(x => x.KullaniciKA == k.KullaniciKA && x.KullaniciSF == k.KullaniciSF);
            if (dene != null)
            {
                Session["KullaniciID"] = dene.KullaniciID;
                Session["KullaniciAdi"] = dene.KullaniciAdi;
                return RedirectToAction("Index", "Hepsisurada");
            }
            else
            {
                TempData["giris"] = "<script>alert('Kullanıcı Adı Veya Şifre Yanlış!');</script>";
                return View();
            }
        }
        public ActionResult Cikis()
        {
            Session["KullaniciID"] = null;
            Session["KullaniciAdi"] = null;
            return RedirectToAction("Index", "Hepsisurada");
        }
        public ActionResult Kaydol()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Kaydol([Bind(Include = "KullaniciID,KullaniciKA,KullaniciSF,KullaniciAdi,KullaniciEmail,KullaniciTelefon,KullaniciDogum,KullaniciAdres")]Kullanicilar kullanicilar)
        {
            var dene = (from i in db.Kullanicilars where i.KullaniciKA == kullanicilar.KullaniciKA select i).FirstOrDefault();
            if (dene == null)
            {
                if (ModelState.IsValid)
                {
                    Session["KullaniciID"] = kullanicilar.KullaniciID;
                    Session["KullaniciAdi"] = kullanicilar.KullaniciAdi;
                    db.Kullanicilars.Add(kullanicilar);
                    db.SaveChanges();
                    TempData["onay"] = "<script>alert('Kaydınız Oluşturuldu!');</script>";
                    return RedirectToAction("Index", "Hepsisurada");
                }
            }
            else { TempData["onay"] = "<script>alert('Bu Kullanıcı Adı Kullanılıyor!');</script>"; }
            return View();
        }
        public ActionResult Urun(int id)
        {
            Urunler u = db.Urunlers.Find(id);
            return View(u);
        }
        public ActionResult Siparislerim()
        {
            try
            {
                int id = int.Parse(Session["KullaniciID"].ToString());
                List<Siparisler> siparisler = (from i in db.Siparislers where i.KullaniciID == id select i).ToList();
                ViewBag.siparisler = siparisler;
                return View();
            }
            catch { return RedirectToAction("Giris", "Hepsisurada"); }
            
        }
        public ActionResult Sepet()
        {
             if (Session["KullaniciID"] == null) return RedirectToAction("Giris", "Hepsisurada");

             int id = int.Parse(Session["KullaniciID"].ToString());
             List<Sepet> Sepets = (from i in db.Sepets where i.KullaniciID==id select i).ToList();
             ViewBag.sepet = Sepets;
             ViewBag.adres = (from i in db.Kullanicilars where i.KullaniciID == id select i.KullaniciAdres).FirstOrDefault();
             return View();
        }
        public ActionResult Odeme(string adres)
        {
            if (Session["KullaniciID"] != null)
            {
                try
                {
                    int id = int.Parse(Session["KullaniciID"].ToString());
                    List<Sepet> Sepets = (from i in db.Sepets where i.KullaniciID == id select i).ToList();
                    foreach (var i in Sepets)
                    {
                        Siparisler siparis = new Siparisler();
                        siparis.KullaniciID = id;
                        siparis.SiparisAdi = i.Urunler.UrunAdi;
                        siparis.SiparisFiyat = i.Urunler.UrunFiyat;
                        siparis.SiparisAdet = i.SepetUrunSayi;
                        siparis.SiparisTarih = DateTime.Now;
                        siparis.SiparisResim = i.Urunler.UrunResim;
                        siparis.SiparisAdres = adres;
                        Urunler urunler = (from u in db.Urunlers where u.UrunID == i.UrunID select u).FirstOrDefault();
                        if ((urunler.UrunStok - i.SepetUrunSayi) < 0)
                        {
                            TempData["test"]= "<script>alert('Ürün Stokta Kalmadı!');</script>";
                            return RedirectToAction("Sepet", "Hepsisurada");
                        }
                        else
                        {
                            urunler.UrunStok -= i.SepetUrunSayi;
                            db.Siparislers.Add(siparis);
                            db.Entry(urunler);
                            db.SaveChanges();
                            TempData["test"] = "<script>alert('Satın Alım Başarılı!');</script>";
                        }
                    }
                    foreach (var item in Sepets)
                    {
                        Sepet sil = (from i in db.Sepets where i.KullaniciID == id select i).FirstOrDefault();
                        db.Sepets.Remove(sil);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Sepet", "Hepsisurada");
                }
                catch
                {
                    TempData["test"] = "<script>alert('Sepet Boş!');</script>";
                    return RedirectToAction("index", "Hepsisurada");
                }
                
            }
            return RedirectToAction("Giris","Hepsisurada");
        }
        public ActionResult SepeteEkle(int id)
        {
            if (Session["KullaniciID"] != null)
            {
                int kulid = int.Parse(Session["KullaniciID"].ToString());
                var dene = (from i in db.Sepets where i.UrunID == id && i.KullaniciID == kulid select i).FirstOrDefault();
                if (dene == null)
                {
                    var fiyat = (from i in db.Urunlers where i.UrunID == id select i.UrunFiyat).FirstOrDefault();
                    Sepet Ekle = new Sepet();
                    Ekle.KullaniciID = kulid;
                    Ekle.UrunID = id;
                    Ekle.SepetUrunSayi = 1;
                    Ekle.SepetToplamFiyat = fiyat;
                    db.Sepets.Add(Ekle);
                    db.SaveChanges();
                }
                return RedirectToAction("Sepet", "Hepsisurada");
            }
            return RedirectToAction("Giris", "Hepsisurada");
        }
        public ActionResult SepetSil(int id)
        {
            try
            {
                int kulid = int.Parse(Session["KullaniciID"].ToString());
                Sepet urun = (from i in db.Sepets where i.UrunID == id && i.KullaniciID == kulid select i).FirstOrDefault();
                db.Sepets.Remove(urun);
                db.SaveChanges();
                return RedirectToAction("Sepet", "Hepsisurada");
            }
            catch { return RedirectToAction("Giris", "Hepsisurada"); }
        }
        public ActionResult SepetAdetGuncelle(int id,int adet)
        {
            try
            {
            int kulid = int.Parse(Session["KullaniciID"].ToString());
            Sepet bul = (from i in db.Sepets where i.SepetID == id && i.KullaniciID==kulid select i).FirstOrDefault();
            bul.SepetUrunSayi = adet;
            bul.SepetToplamFiyat = bul.Urunler.UrunFiyat * adet;
            db.Entry(bul);
            db.SaveChanges();
            return RedirectToAction("Sepet", "Hepsisurada");

            }
            catch { return RedirectToAction("Giris", "Hepsisurada"); }
        }
        public ActionResult Hesabim()
        {
            if (Session["KullaniciID"] != null)
            {
                int kulid = int.Parse(Session["KullaniciID"].ToString());
                Kullanicilar kullanici = db.Kullanicilars.Find(kulid);
                return View(kullanici);
            }
            return RedirectToAction("Giris", "Hepsisurada");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Hesabim([Bind(Include = "KullaniciID,KullaniciKA,KullaniciSF,KullaniciAdi,KullaniciEmail,KullaniciTelefon,KullaniciDogum,KullaniciAdres")]Kullanicilar kullanici)
        {
            int kulid = int.Parse(Session["KullaniciID"].ToString());
            Kullanicilar dene = (from i in db.Kullanicilars where i.KullaniciKA == kullanici.KullaniciKA && i.KullaniciID != kulid select i).FirstOrDefault();
            if (dene == null)
            {
                if (ModelState.IsValid)
                {
                    Kullanicilar degis = db.Kullanicilars.Find(kulid);
                    degis.KullaniciKA = kullanici.KullaniciKA;
                    degis.KullaniciSF = kullanici.KullaniciSF;
                    degis.KullaniciAdi = kullanici.KullaniciAdi;
                    degis.KullaniciEmail = kullanici.KullaniciEmail;
                    degis.KullaniciTelefon = kullanici.KullaniciTelefon;
                    degis.KullaniciDogum = kullanici.KullaniciDogum;
                    degis.KullaniciAdres = kullanici.KullaniciAdres;
                    db.Entry(degis);
                    db.SaveChanges();
                    Session["KullaniciAdi"] = kullanici.KullaniciAdi;
                    TempData["onay"] = "<script>alert('Hesabınız Güncellendi!');</script>";
                    return RedirectToAction("Hesabim", "Hepsisurada");
                }
            }
            else TempData["msg"] = "<script>alert('Bu Kullanıcı Adı Kullanılıyor!');</script>";
            return View(kullanici);
        }
    }
}