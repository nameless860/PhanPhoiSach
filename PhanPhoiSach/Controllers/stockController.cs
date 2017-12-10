using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhanPhoiSach.Models;

namespace PhanPhoiSach.Controllers
{
    public class stockController : Controller
    {
        private Model1 db = new Model1();

        [HttpGet]
        public ActionResult Thongke()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Thongke(FormCollection collection)
        {
            DateTime date = DateTime.Parse(collection["date"]);
            Session["date"] = date;  
            date = date.AddDays(1);
            
            Session["tonkho"] = new List<store>();   // Tạo một phiên làm việc để chứa danh sách TỒN KHO sách theo thời gian
            var books_id = (from b in db.books select b.book_id);  // Lấy ID của tất cả các sách hiện có trong công ty
            foreach (var b in books_id.ToList())
            {
                if (db.stores.OrderByDescending(a => a.store_time).FirstOrDefault(m => m.fk_book == b && m.store_time <= date) != null)  // Nếu sách có tồn kho và thỏa thời gian thì thêm vào Session
                {
                    ((List<store>)Session["tonkho"]).Add(db.stores.OrderByDescending(a => a.store_time).FirstOrDefault(m => m.fk_book == b && m.store_time <= date));
                }
                else  // Không thỏa thì tồn kho của sách đó = 0 và thêm vào Session
                {
                    store s = new store();
                    s.book = db.books.Find(b);
                    s.store_quantity = 0;
                    ((List<store>)Session["tonkho"]).Add(s);
                }
            }

            return View();
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
