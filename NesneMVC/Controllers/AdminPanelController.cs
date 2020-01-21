using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NesneMVC.Models;
using NesneMVC.Veri;
using System.Data.Entity;
using System.Net;

namespace NesneMVC.Controllers
{
    public class AdminPanelController : Controller
    {
        Context db = new Context();

        public ActionResult Cikis()
        {
            Session["YoneticiID"] = null;
            Session["YoneticiAdi"] = null;
            return RedirectToAction("Giris", "AdminPanel");
        }
        public ActionResult Hesabim()
        {
            try
            {
                int id = int.Parse(Session["YoneticiID"].ToString());
                Yoneticiler yoneticiler = db.Yoneticilers.Find(id);
                if (yoneticiler == null)
                {
                    return HttpNotFound();
                }
                return View(yoneticiler);
            }
            catch { return View(); }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Hesabim([Bind(Include = "YoneticiID,YoneticiKA,YoneticiSF,YoneticiAdi,YoneticiEmail,YoneticiTelefon,YoneticiDogum,YoneticiAdres")] Yoneticiler yoneticiler)
        {
            Yoneticiler dene = (from i in db.Yoneticilers where i.YoneticiKA == yoneticiler.YoneticiKA && i.YoneticiID != yoneticiler.YoneticiID select i).FirstOrDefault();
            if (dene == null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(yoneticiler).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["onay"] = "<script>alert('Hesabınız Güncellendi!');</script>";
                    Session["YoneticiAdi"] = yoneticiler.YoneticiAdi;
                    return View(yoneticiler);
                }
            }
            else TempData["hata"] = "<script>alert('Bu Kullanıcı Adı Kullanılıyor!');</script>";
            return View(yoneticiler);
        }
        public ActionResult Giris()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Giris(Yoneticiler y)
        {
            var dene = db.Yoneticilers.FirstOrDefault(x => x.YoneticiKA == y.YoneticiKA && x.YoneticiSF == y.YoneticiSF);
            if (dene != null)
            {
                Session["YoneticiID"] = dene.YoneticiID;
                Session["YoneticiAdi"] = dene.YoneticiAdi;
                return RedirectToAction("Index", "AdminUrunler");
            }
            else
            {
                TempData["giris"] = "<script>alert('Kullanıcı Adı Veya Şifre Yanlış!');</script>";
                return View();
            }
        }
        public ActionResult HesapEkle()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HesapEkle([Bind(Include = "YoneticiID,YoneticiKA,YoneticiSF,YoneticiAdi,YoneticiEmail,YoneticiTelefon,YoneticiDogum,YoneticiAdres")] Yoneticiler yoneticiler)
        {
            Yoneticiler dene = (from i in db.Yoneticilers where i.YoneticiKA == yoneticiler.YoneticiKA select i).FirstOrDefault();
            if (dene == null)
            {
                if (ModelState.IsValid)
                {
                    db.Yoneticilers.Add(yoneticiler);
                    db.SaveChanges();
                    TempData["onay"] = "<script>alert('Yönetici Kaydı Oluşturuldu!');</script>";
                    return RedirectToAction("Index", "AdminUrunler");
                }
            }
            else TempData["msg"] = "<script>alert('Bu Kullanıcı Adı Kullanılıyor!');</script>";
            return View();
        }
        public ActionResult Galeri()
        {
            var galeri = db.Galeris;
            return View(galeri);
        }
        public ActionResult GaleriGuncelle(int id)
        {
            Galeri galeri = db.Galeris.Find(id);
            return View(galeri);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GaleriGuncelle([Bind(Include = "GaleriID,GaleriBaslik,GaleriDuyuru,GaleriResim")] Galeri galeri, HttpPostedFileBase UploadImage)
        {
            if (ModelState.IsValid)
            {
                if (UploadImage == null)
                {
                    db.Entry(galeri).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    if (UploadImage.ContentType == "image/jpg" || UploadImage.ContentType == "image/png" || UploadImage.ContentType == "image/jpeg")
                    {
                        UploadImage.SaveAs(Server.MapPath("/Resimler/" + UploadImage.FileName));
                        galeri.GaleriResim = "/Resimler/" + UploadImage.FileName;

                        db.Entry(galeri).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Galeri", "AdminPanel");
        }
        public ActionResult GaleriSil(int? id)
        {
            var bul = db.Galeris.Find(id);
            return View(bul);
        }
        [HttpPost]
        public ActionResult GaleriSil(int id)
        {
            Galeri bul = (from i in db.Galeris where i.GaleriID == id select i).FirstOrDefault();
            db.Galeris.Remove(bul);
            db.SaveChanges();
            return RedirectToAction("Galeri", "AdminPanel");
        }
        public ActionResult GaleriEkle()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GaleriEkle([Bind(Include = "GaleriID,GaleriBaslik,GaleriDuyuru")] Galeri galeri, HttpPostedFileBase UploadImage)
        {
            if (ModelState.IsValid)
            {
                if (UploadImage != null)
                {
                    if (UploadImage.ContentType == "image/jpg" || UploadImage.ContentType == "image/png" || UploadImage.ContentType == "image/jpeg")
                    {
                        UploadImage.SaveAs(Server.MapPath("/Resimler/" + UploadImage.FileName));
                        galeri.GaleriResim = "/Resimler/" + UploadImage.FileName;

                        db.Galeris.Add(galeri);
                        db.SaveChanges();
                        return RedirectToAction("Galeri","AdminPanel");
                    }
                }
                else TempData["bos"] = "<script>alert('Resim Yüklemeniz Şarttır!');</script>";
            }
            return View(galeri);
        }
        public ActionResult Siparisler()
        {
            var siparis = db.Siparislers;
            return View(siparis);
        }
    }
}