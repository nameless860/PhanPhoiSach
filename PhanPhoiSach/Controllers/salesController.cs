using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhanPhoiSach.Models;
using PhanPhoiSach.ViewModels;

namespace PhanPhoiSach.Controllers
{
    public class salesController : Controller
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
            DateTime date2 = DateTime.Parse(collection["date2"]);
            Session["date"] = date;
            Session["date2"] = date2;
            date = date.AddDays(1);
            date2 = date2.AddDays(1);

            // Dùng linq để tìm doanh thu thu được từ mỗi sách ( sử dụng View Model revenue_statistics để chứa thông tin )
            var tk_doanhthu = from spi in db.sale_payment_item
                              from b in db.books
                              where (from sp in db.sale_payment
                                     where sp.sale_payment_time >= date && sp.sale_payment_time <= date2
                                     select sp.sale_payment_id).Contains(spi.fk_sale_payment) && spi.fk_book == b.book_id
                              group new { spi, b } by new { spi.fk_book, b.book_name } into spi_group
                              select new revenue_statistics
                              {
                                  book_id = spi_group.Key.fk_book,
                                  book_name = spi_group.Key.book_name,
                                  quantity = spi_group.Sum(groupItem => groupItem.spi.sale_payment_item_quantity),
                                  total_money = spi_group.Sum(groupItem => groupItem.spi.sale_payment_item_money)
                              };
            
            decimal? tongdoanhthu = 0;  // tính tổng doanh thu
            foreach(var tk in tk_doanhthu)
            {
                tongdoanhthu += tk.total_money;
            }

            Session["tk_doanhthu"] = tk_doanhthu.ToList();
            ViewBag.Tongdoanhthu = tongdoanhthu;
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