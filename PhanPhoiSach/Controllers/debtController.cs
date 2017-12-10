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
    public class debtController : Controller
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
            Session["date"] = date.Date;
            date = date.AddDays(1);

            string ChonNo = collection["ChonNo"].ToString();   // Chọn nợ: ĐẠI LÝ hay NHÀ XUẤT BẢN từ Radio button trên View
            if(ChonNo == "DL")              // Công nợ đại lý
            {
                Session["loai"] = "DL";     // Để hiển thị nợ của Đại lý hay Nhà Xuất Bản trên View Thongke
                Session["NoDaiLy"] = new List<agency_debt>();    //     Phiên làm việc để chứa thống kê nợ ĐẠI LÝ
                var agencies_id = (from a in db.agencies select a.agency_id);       // Lấy ID của tất cả đại lý
                foreach (var a_id in agencies_id.ToList())
                {
                    if (db.agency_debt.OrderByDescending(a => a.agency_debt_time).FirstOrDefault(m => m.fk_agency == a_id && m.agency_debt_time <= date) != null)   // Nếu đại lý này có tồn tại nợ và thỏa thời gian thì thêm vào Session
                    {
                        ((List<agency_debt>)Session["NoDaiLy"]).Add(db.agency_debt.OrderByDescending(a => a.agency_debt_time).FirstOrDefault(m => m.fk_agency == a_id && m.agency_debt_time <= date));
                    }
                    else  // không thỏa thì nợ của Đại lý này = 0 và thêm vào Session
                    {
                        agency_debt ad = new agency_debt();
                        ad.agency = db.agencies.Find(a_id);
                        ad.agency_debt_total_money = 0;
                        ((List<agency_debt>)Session["NoDaiLy"]).Add(ad);
                       
                    }
                }
            }
            else                        // Công nợ nhà xuất bản
            {
                Session["loai"] = "NXB";        // Để hiển thị nợ của Đại lý hay Nhà Xuất Bản trên View Thongke
                Session["NoNXB"] = new List<supplier_debt>();   //     Phiên làm việc để chứa thống kê nợ NXB
                var suppliers_id = (from s in db.suppliers select s.supplier_id);       // Lấy ID của tất cả NXB
                foreach (var s_id in suppliers_id.ToList())
                {
                    if (db.supplier_debt.OrderByDescending(a => a.supplier_debt_time).FirstOrDefault(m => m.fk_supplier == s_id && m.supplier_debt_time <= date) != null)   // Nếu NXB này có tồn tại nợ và thỏa thời gian thì thêm vào Session
                    {
                        ((List<supplier_debt>)Session["NoNXB"]).Add(db.supplier_debt.OrderByDescending(a => a.supplier_debt_time).FirstOrDefault(m => m.fk_supplier == s_id && m.supplier_debt_time <= date));
                        
                    }
                    else    // không thỏa thì nợ của NXB này = 0 và thêm vào Session
                    {
                        supplier_debt sd = new supplier_debt();
                        sd.supplier = db.suppliers.Find(s_id);
                        sd.supplier_debt_total_money = 0;
                        ((List<supplier_debt>)Session["NoNXB"]).Add(sd);
                    }
                }
            }
            return View();
        }

        public ActionResult Details_agency(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            agency_debt agency_debt = db.agency_debt.Find(id);
            if (agency_debt == null)        
            {
                return View();          // Nếu đại lý này không có nợ
            }
            return View(agency_debt);       // Nếu đại lý này có nợ
        }

        public ActionResult Details_supplier(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            supplier_debt supplier_debt = db.supplier_debt.Find(id);
            if (supplier_debt == null)
            {
                return View();      // Nếu NXB này không có nợ
            }
            return View(supplier_debt);     // Nếu NXB này có nợ
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
