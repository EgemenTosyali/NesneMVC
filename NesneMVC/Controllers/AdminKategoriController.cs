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

namespace NesneMVC.Controllers
{
    public class AdminKategoriController : Controller
    {
        private Context db = new Context();
        
        public ActionResult Index()
        {
            return View(db.Kategoris.ToList());
        }
        public ActionResult KategoriEkle()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KategoriEkle([Bind(Include = "KategoriID,KategoriAdi")] Kategori kategori)
        {
            Kategori dene = (from i in db.Kategoris where i.KategoriAdi == kategori.KategoriAdi select i).FirstOrDefault();
            if (dene == null)
            {
                if (ModelState.IsValid)
                {
                    db.Kategoris.Add(kategori);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else TempData["msg"] = "<script>alert('Bu Kategori Adı Zaten Var!');</script>";
            return View(kategori);
        }
        
        public ActionResult Guncelle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kategori kategori = db.Kategoris.Find(id);
            if (kategori == null)
            {
                return HttpNotFound();
            }
            return View(kategori);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Guncelle([Bind(Include = "KategoriID,KategoriAdi")] Kategori kategori)
        {
            Kategori dene = (from i in db.Kategoris where i.KategoriAdi == kategori.KategoriAdi select i).FirstOrDefault();
            if (dene == null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(kategori).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else TempData["msg"] = "<script>alert('Bu Kategori Adı Zaten Var!');</script>";
            return View(kategori);
        }
        public ActionResult Sil(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kategori kategori = db.Kategoris.Find(id);
            if (kategori == null)
            {
                return HttpNotFound();
            }
            return View(kategori);
        }

        [HttpPost, ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public ActionResult Sil(int id)
        {
            Kategori kategori = db.Kategoris.Find(id);
            Urunler urunler = (from i in db.Urunlers where i.KategoriID == id select i).FirstOrDefault();
            Sepet sepetler = (from i in db.Sepets where i.Urunler.KategoriID == id select i).FirstOrDefault();
            db.Kategoris.Remove(kategori);
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
