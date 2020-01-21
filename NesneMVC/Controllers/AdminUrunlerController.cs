using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NesneMVC.Models;
using NesneMVC.Veri;
using PagedList;
using PagedList.Mvc;

namespace NesneMVC.Controllers
{
    public class AdminUrunlerController : Controller
    {
        private Context db = new Context();
        public ActionResult Index(int sayfa=1 ,string arama=null)
        {
            List<Kategori> Kategoris = db.Kategoris.ToList();
            ViewBag.kategori = Kategoris;

            var Urunlers = from u in db.Urunlers select u;
            if (!string.IsNullOrEmpty(arama))
            {
                Urunlers = Urunlers.Where(u => u.UrunAdi.Contains(arama));
            }
            return View(Urunlers.ToList().ToPagedList(sayfa,5));                        
        }
        public ActionResult Detay(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urunler urunler = db.Urunlers.Find(id);
            if (urunler == null)
            {
                return HttpNotFound();
            }
            return View(urunler);
        }
        
        public ActionResult UrunEkle()
        {
            ViewBag.KategoriID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UrunEkle([Bind(Include = "UrunID,KategoriID,UrunAdi,UrunFiyat,UrunStok,UrunBilgi")] Urunler urunler,HttpPostedFileBase UploadImage)
        {
            Urunler dene = (from i in db.Urunlers where i.UrunAdi == urunler.UrunAdi select i).FirstOrDefault();
            if (dene == null)
            {
                if (ModelState.IsValid)
                {
                    if (UploadImage != null)
                    {
                        if (UploadImage.ContentType == "image/jpg" || UploadImage.ContentType == "image/png" || UploadImage.ContentType == "image/jpeg")
                        {
                            UploadImage.SaveAs(Server.MapPath("/Resimler/" + UploadImage.FileName));
                            urunler.UrunResim = "/Resimler/" + UploadImage.FileName;

                            db.Urunlers.Add(urunler);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    else TempData["bos"]= "<script>alert('Resim Yüklemeniz Şarttır!');</script>";
                }
            }
            else TempData["msg"] = "<script>alert('Bu Ürün Adı Kullanılıyor!');</script>";

            ViewBag.KategoriID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi", urunler.KategoriID);
            return View(urunler);
        }
        public ActionResult Kategori(int id,string arama)
        {
                ViewBag.ID = id;
                var Urunlers = from u in db.Urunlers where u.KategoriID == id select u;
                if (!string.IsNullOrEmpty(arama))
                {
                    Urunlers = Urunlers.Where(u => u.UrunAdi.Contains(arama));
                }
                return View(Urunlers.ToList());
        }
        public ActionResult KategoriyeEkle(int id)
        {
            var kategoriad=(from i in db.Kategoris where i.KategoriID==id select i.KategoriAdi).FirstOrDefault();
            ViewBag.kategoriid = id;
            ViewBag.kategoriad = kategoriad;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KategoriyeEkle([Bind(Include = "UrunID,KategoriID,UrunAdi,UrunFiyat,UrunStok,UrunBilgi")] Urunler urunler,int id, HttpPostedFileBase UploadImage)
        {
            Urunler dene = (from i in db.Urunlers where i.UrunAdi == urunler.UrunAdi select i).FirstOrDefault();
            if (dene == null)
            {
                if (ModelState.IsValid)
                {
                    if (UploadImage != null)
                    {
                        if (UploadImage.ContentType == "image/jpg" || UploadImage.ContentType == "image/png" || UploadImage.ContentType == "image/jpeg")
                        {
                            UploadImage.SaveAs(Server.MapPath("/Resimler/" + UploadImage.FileName));
                            urunler.UrunResim = "/Resimler/" + UploadImage.FileName;
                            urunler.KategoriID = id;

                            db.Urunlers.Add(urunler);
                            db.SaveChanges();
                            return RedirectToAction("Kategori", new { id = urunler.KategoriID });
                        }
                    }
                    else TempData["bos"] = "<script>alert('Resim Yüklemeniz Şarttır!');</script>";
                }
            }
            else TempData["msg"] = "<script>alert('Bu Ürün Adı Kullanılıyor!');</script>";

            ViewBag.KategoriID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi", urunler.KategoriID);
            return View(urunler);
        }

        public ActionResult Guncelle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urunler urunler = db.Urunlers.Find(id);
            if (urunler == null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi", urunler.KategoriID);
            return View(urunler);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Guncelle([Bind(Include = "UrunID,KategoriID,UrunAdi,UrunFiyat,UrunStok,UrunBilgi,UrunResim")] Urunler urunler,HttpPostedFileBase UploadImage)
        {
            Urunler dene = (from i in db.Urunlers where i.UrunAdi == urunler.UrunAdi && i.UrunID!=urunler.UrunID select i).FirstOrDefault();
            if(dene==null)
            {
                if (ModelState.IsValid)
                {
                    if (UploadImage == null)
                    {
                        db.Entry(urunler).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        if (UploadImage.ContentType == "image/jpg" || UploadImage.ContentType == "image/png" || UploadImage.ContentType == "image/jpeg")
                        {
                            UploadImage.SaveAs(Server.MapPath("/Resimler/" + UploadImage.FileName));
                            urunler.UrunResim = "/Resimler/" + UploadImage.FileName;

                            db.Entry(urunler).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    return RedirectToAction("Index");
                }
            }
            else TempData["msg"] = "<script>alert('Bu Ürün Adı Kullanılıyor!');</script>";
            ViewBag.KategoriID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi", urunler.KategoriID);
            return View(urunler);
        }
        public ActionResult Sil(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urunler urunler = db.Urunlers.Find(id);
            if (urunler == null)
            {
                return HttpNotFound();
            }
            return View(urunler);
        }
        [HttpPost, ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public ActionResult Sil(int id)
        {
            Urunler urunler = (from i in db.Urunlers where i.UrunID == id select i).FirstOrDefault();
            Sepet sepetler = (from i in db.Sepets where i.UrunID == id select i).FirstOrDefault();
            try
            {
                db.Sepets.Remove(sepetler);
                db.Urunlers.Remove(urunler);
            } catch { }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
