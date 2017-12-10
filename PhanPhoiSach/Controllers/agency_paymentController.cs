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
    public class agency_paymentController : Controller
    {
        private Model1 db = new Model1();

        public ActionResult Index()
        {
            var sale_payment = db.sale_payment.Include(s => s.agency);      //  Load danh sách phiếu thanh toán đại lý lên View
            return View(sale_payment.ToList());
        }

        public ActionResult Deletectpx(int bookid)
        {
            var list = (List<sale_payment_item>)Session["ctPhieuThanhToanDL"];
            list.RemoveAll(p => p.fk_book == bookid);
            return RedirectToAction("ThemChiTietPhieuThanhToanDL");
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sale_payment sale_payment = db.sale_payment.Find(id);       // tìm chi tiết phiếu thanh toán đại lý theo ID
            if (sale_payment == null)
            {
                return HttpNotFound();
            }
            return View(sale_payment);
        }

        public ActionResult Create()
        {
            ViewBag.fk_agency = new SelectList(db.agencies, "agency_id", "agency_name");        // Load danh sách các Đại lý lên View
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sale_payment_id,fk_agency,sale_payment_time,sale_payment_money_pay")] sale_payment sale_payment)
        {
            if (ModelState.IsValid)
            {
                sale_payment.sale_payment_time = DateTime.Now;
                Session["PhieuThanhToanDL"] = sale_payment;        // Tạo 1 phiên làm việc để chứa phiếu thanh toán của đại lý
                Session["ctPhieuThanhToanDL"] = new List<sale_payment_item>();     // Tạo 1 phiên làm việc để chứa chi tiết phiếu thanh toán của đại lý
                return RedirectToAction("ThemChiTietPhieuThanhToanDL");
            }

            ViewBag.fk_agency = new SelectList(db.agencies, "agency_id", "agency_name", sale_payment.fk_agency);
            return View(sale_payment);
        }

        [HttpGet]
        public ActionResult ThemChiTietPhieuThanhToanDL()
        {

            sale_payment sp = (sale_payment)Session["PhieuThanhToanDL"];
            agency_debt debt = db.agency_debt.OrderByDescending(m => m.agency_debt_time).FirstOrDefault(k => k.fk_agency == sp.fk_agency); // Tìm nợ mới nhất của Đại lý này
            List<book> books = new List<book>();  // Tạo danh sách để chứa các sách mà Đại lý đang nợ ( dùng cho Select List trên View )
            if(debt != null)        // Nếu có nợ
            {
                var fk_book = (from b in db.agency_debt_item
                               where b.fk_agency_debt == debt.agency_debt_id && b.agency_debt_item_quantity > 0
                               select b.fk_book);
                foreach (var fk in fk_book)
                {
                    books.Add(db.books.Find(fk));
                }
            }
            ViewBag.sach = new SelectList(books, "book_id", "book_name");
            return View();
        }

        public ActionResult ThemChiTietPhieuThanhToanDL(FormCollection chitiet)
        {
            sale_payment sp = (sale_payment)Session["PhieuThanhToanDL"];
            agency_debt d= db.agency_debt.OrderByDescending(m => m.agency_debt_time).FirstOrDefault(k => k.fk_agency == sp.fk_agency);
            List<book> books = new List<book>();
            if(d != null)
            {
                var fk_book = (from b in db.agency_debt_item
                               where b.fk_agency_debt == d.agency_debt_id && b.agency_debt_item_quantity>0
                               select b.fk_book);
                foreach (var fk in fk_book)
                {
                    books.Add(db.books.Find(fk));
                }
            }
            ViewBag.sach = new SelectList(books, "Book_id", "Book_name");
            ViewBag.loi = null;

            if (Request.Form["add"] != null)        // Nếu Click nút THÊM trên View ( thêm một sách mới vào CT phiếu )
            {
                if (ModelState.IsValid)
                {
                    bool check = true;
                    // Kiểm tra xem SÁCH này đã được thêm trước đó trong chi tiết phiếu chưa
                    foreach (var ctpttdl in (List<sale_payment_item>)Session["ctPhieuThanhToanDL"])
                    {
                        if (ctpttdl.fk_book == Int32.Parse(chitiet["sach"].ToString()))
                        {
                            check = false;
                            ViewBag.loi = "Sách đã được thêm vào phiếu trước đó";
                            break;
                        }
                    }

                    // Nếu sách chưa được thêm thì kiểm tra tiếp như sau:
                    if (check)
                    {
                        //kiểm tra trong kho có sách đó hay không
                        if (chitiet["sach"] == null)
                        {
                            ViewBag.loi = "Không tồn tại sách để thanh toán";
                            goto baoloi;
                        }

                        //kiểm tra mã sách trong CSDL xem có tồn tại không
                        var sach = db.books.Find(Int32.Parse(chitiet["sach"].ToString()));                      
                        if (sach == null)
                        {
                            ViewBag.loi = "Mã sách không tồn tại";
                            goto baoloi;
                        }
                        else
                        {       // Thêm chi tiết mới vào Session["ctPhieuThanhToanDL"]
                                sale_payment_item ctpttdl = new sale_payment_item();
                                ctpttdl.fk_sale_payment = (db.sale_payment.Max(u => (int?)u.sale_payment_id) != null ? db.sale_payment.Max(u => u.sale_payment_id) : 0) + 1;
                                ctpttdl.fk_book = Int32.Parse(chitiet["sach"].ToString());
                                ctpttdl.book = db.books.Find(Int32.Parse(chitiet["sach"].ToString()));
                                ctpttdl.sale_payment_item_quantity = Int32.Parse(chitiet["soluong"].ToString());
                                ctpttdl.sale_payment_item_price = decimal.Parse(db.books.Find(ctpttdl.fk_book).book_seling_price.ToString());
                                ctpttdl.sale_payment_item_money = ctpttdl.sale_payment_item_quantity * ctpttdl.sale_payment_item_price;

                                if(ctpttdl.sale_payment_item_quantity <= 0)
                                {
                                    ViewBag.loi = "Số lượng thanh toán phải lớn hơn 0";
                                    goto baoloi;
                                }
                                
                                // Kiểm tra xem có thanh toán hơn nợ mà đang thiếu không
                                int? checksoluong = db.agency_debt_item.FirstOrDefault(m => m.fk_agency_debt == d.agency_debt_id && m.fk_book == ctpttdl.fk_book).agency_debt_item_quantity;
                                if(ctpttdl.sale_payment_item_quantity <= checksoluong)
                                {
                                    ((List<sale_payment_item>)Session["ctPhieuThanhToanDL"]).Add(ctpttdl);
                                }
                                else
                                {
                                    ViewBag.loi = "Vượt quá số lượng nợ hiện tại, số lượng nợ hiện tại là: " + checksoluong;
                                    goto baoloi;
                                }     
                        }
                        return RedirectToAction("ThemChiTietPhieuThanhToanDL");
                    }
                }
            }


            else if (Request.Form["create"] != null)        // Nếu Click nút LƯU PHIẾU trên View
            {
                if (ModelState.IsValid)
                {
                    if (((List<sale_payment_item>)Session["ctPhieuThanhToanDL"]).Count == 0)
                    {
                        ViewBag.loi = "Không được để phiếu trống";
                        goto baoloi;
                    }

                    var luu = db.sale_payment.Add((sale_payment)Session["PhieuThanhToanDL"]);   // Lưu phiếu thanh toán ( chưa ghi tổng tiền phiếu )
                    db.SaveChanges();

                    decimal tongTien = 0;
                    decimal? temptongtien = 0;

                    foreach (var ctpttdl in (List<sale_payment_item>)Session["ctPhieuThanhToanDL"])
                    {
                        temptongtien += decimal.Parse((ctpttdl.sale_payment_item_price * ctpttdl.sale_payment_item_quantity).ToString());
                        tongTien = decimal.Parse(temptongtien.ToString());

                        //lưu chi tiết phiếu xuất
                        sale_payment_item ctPhieuThanhToanDL = new sale_payment_item();
                        ctPhieuThanhToanDL = ctpttdl;
                        ctPhieuThanhToanDL.book = null;
                        db.sale_payment_item.Add(ctPhieuThanhToanDL);           
                    }

                    //lưu phiếu thanh toán
                    agency_debt debt1 = db.agency_debt.OrderByDescending(m => m.agency_debt_id).FirstOrDefault(m => m.fk_agency == (int)luu.fk_agency);
                    if (debt1 == null || (debt1 != null && debt1.agency_debt_total_money > 0 && debt1.agency_debt_total_money >= tongTien) || debt1.agency_debt_total_money == 0)
                    {
                        // cập nhật tổng tiền phiếu cho phiếu
                        luu.sale_payment_money_pay = tongTien;
                        db.sale_payment.Attach(luu);
                        db.Entry(luu).State = EntityState.Modified;
                        db.SaveChanges();

                        //cập nhật nợ Đại Lý
                        agency_debt debt = new agency_debt();
                        debt.agency_debt_time = DateTime.Now;
                        debt.fk_agency = luu.fk_agency;
                        debt.agency_debt_total_money = debt1.agency_debt_total_money - luu.sale_payment_money_pay;
                        
                        db.agency_debt.Add(debt);

                        //cập nhật CHI TIẾT NỢ đại lý:
                        //// Thêm các chi tiết đã nợ trước đó nhưng không có trong Session["ctPhieuThanhToanDL"]
                        int ad_id = (db.agency_debt.Max(u => u.agency_debt_id)) + 1;
                        agency_debt ad = db.agency_debt.OrderByDescending(a => a.agency_debt_time).FirstOrDefault(a => a.fk_agency == debt.fk_agency);
                        List<agency_debt_item> list = ad.agency_debt_item.ToList();
                        foreach (var ctno in list)
                        {
                            int check = 1;
                            foreach (var ctpttdl in (List<sale_payment_item>)Session["ctPhieuThanhToanDL"])
                            {
                                if (ctno.fk_book == ctpttdl.fk_book)
                                {
                                    check = 0;
                                    break;
                                }
                            }
                            if (check == 1)
                            {
                                agency_debt_item adi = ctno;
                                adi.fk_agency_debt = ad_id;
                                db.agency_debt_item.Add(adi);
                            }
                        }

                        //// Thêm các chi tiết nợ mới trong Session["ctPhieuThanhToanDL"]
                        foreach (var ctpttdl in (List<sale_payment_item>)Session["ctPhieuThanhToanDL"])
                        {
                            agency_debt_item adi = new agency_debt_item();
                            adi.fk_agency_debt = ad_id;
                            adi.fk_book = ctpttdl.fk_book;  
                            adi.agency_debt_item_quantity = ad.agency_debt_item.FirstOrDefault(a => a.fk_book == ctpttdl.fk_book).agency_debt_item_quantity - ctpttdl.sale_payment_item_quantity;
                            adi.agency_debt_item_money = ad.agency_debt_item.FirstOrDefault(a => a.fk_book == ctpttdl.fk_book).agency_debt_item_money - ctpttdl.sale_payment_item_money;
                            if(adi.agency_debt_item_quantity > 0 && adi.agency_debt_item_money > 0)
                            {
                                db.agency_debt_item.Add(adi);
                            }
                            db.SaveChanges();
                        }
                        Session["ctPhieuThanhToanDL"] = null;
                        Session["PhieuThanhToanDL"] = null;
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        ViewBag.loi = "Vượt quá số nợ cho phép, hãy bấm hủy phiếu để tạo lại!";
                        goto baoloi;
                    }
                }
            }
        baoloi:
            return View();
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sale_payment sale_payment = db.sale_payment.Find(id);
            if (sale_payment == null)
            {
                return HttpNotFound();
            }
            ViewBag.fk_agency = new SelectList(db.agencies, "agency_id", "agency_name", sale_payment.fk_agency);
            return View(sale_payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "sale_payment_id,fk_agency,sale_payment_time,sale_payment_money_pay")] sale_payment sale_payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sale_payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.fk_agency = new SelectList(db.agencies, "agency_id", "agency_name", sale_payment.fk_agency);
            return View(sale_payment);
        }

        // GET: /sale_payment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sale_payment sale_payment = db.sale_payment.Find(id);
            if (sale_payment == null)
            {
                return HttpNotFound();
            }
            return View(sale_payment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            sale_payment sale_payment = db.sale_payment.Find(id);
            db.sale_payment.Remove(sale_payment);
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
