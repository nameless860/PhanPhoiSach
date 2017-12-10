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
    public class supplier_paymentController : Controller
    {
        private Model1 db = new Model1();

        public ActionResult Index()
        {
            var purchase_payment = db.purchase_payment.Include(s => s.supplier);
            return View(purchase_payment.ToList());
        }

        public ActionResult Deletectpx(int bookid)
        {
            var list = (List<purchase_payment_item>)Session["ctPhieuThanhToanNXB"];
            list.RemoveAll(p => p.fk_book == bookid);
            return RedirectToAction("ThemChiTietPhieuThanhToanNXB");
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            purchase_payment purchase_payment = db.purchase_payment.Find(id);
            if (purchase_payment == null)
            {
                return HttpNotFound();
            }
            return View(purchase_payment);
        }

        public ActionResult Create()
        {
            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "purchase_payment_id,fk_supplier,purchase_payment_time,purchase_payment_money_pay")] purchase_payment purchase_payment)
        {
            if (ModelState.IsValid)
            {
                purchase_payment.purchase_payment_time = DateTime.Now;
                Session["PhieuThanhToanNXB"] = purchase_payment;
                Session["ctPhieuThanhToanNXB"] = new List<purchase_payment_item>();
                return RedirectToAction("ThemChiTietPhieuThanhToanNXB");
            }

            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name", purchase_payment.fk_supplier);
            return View(purchase_payment);
        }

        [HttpGet]
        public ActionResult ThemChiTietPhieuThanhToanNXB()
        {

            purchase_payment sp = (purchase_payment)Session["PhieuThanhToanNXB"];
            supplier_debt debt = db.supplier_debt.OrderByDescending(m => m.supplier_debt_time).FirstOrDefault(k => k.fk_supplier == sp.fk_supplier);
            List<book> books = new List<book>();
            if(debt != null)
            {
                var fk_book = (from b in db.supplier_debt_item
                               where b.fk_supplier_debt == debt.supplier_debt_id && b.supplier_debt_item_quantity>0
                               select b.fk_book);
                foreach (var fk in fk_book)
                {
                    books.Add(db.books.Find(fk));
                }
            }
            ViewBag.sach = new SelectList(books, "book_id", "book_name");
            return View();
        }

        public ActionResult ThemChiTietPhieuThanhToanNXB(FormCollection chitiet)
        {
            purchase_payment sp = (purchase_payment)Session["PhieuThanhToanNXB"];
            supplier_debt d= db.supplier_debt.OrderByDescending(m => m.supplier_debt_time).FirstOrDefault(k => k.fk_supplier == sp.fk_supplier);
            List<book> books = new List<book>();
            if(d != null)
            {
                var fk_book = (from b in db.supplier_debt_item
                               where b.fk_supplier_debt == d.supplier_debt_id && b.supplier_debt_item_quantity>0
                               select b.fk_book);
                foreach (var fk in fk_book)
                {
                    books.Add(db.books.Find(fk));
                }
            }

            ViewBag.sach = new SelectList(books, "Book_id", "Book_name");
            ViewBag.loi = null;

            if (Request.Form["add"] != null)
            {
                if (ModelState.IsValid)
                {
                    bool check = true;
                    // Kiểm tra xem SÁCH này đã được thêm trước đó trong chi tiết phiếu chưa
                    foreach (var ctpttnxb in (List<purchase_payment_item>)Session["ctPhieuThanhToanNXB"])
                    {
                        if (ctpttnxb.fk_book == Int32.Parse(chitiet["sach"].ToString()))
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
                            ViewBag.loi = "Không tồn tại sách để xuất";
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
                        {
                                // Thêm chi tiết mới vào Session["ctPhieuThanhToanNXB"]
                                purchase_payment_item ctpttnxb = new purchase_payment_item();
                                ctpttnxb.fk_purchase_payment = (db.purchase_payment.Max(u => (int?)u.purchase_payment_id) != null ? db.purchase_payment.Max(u => u.purchase_payment_id) : 0) + 1;
                                ctpttnxb.fk_book = Int32.Parse(chitiet["sach"].ToString());
                                ctpttnxb.book = db.books.Find(Int32.Parse(chitiet["sach"].ToString()));
                                ctpttnxb.purchase_payment_item_quantity = Int32.Parse(chitiet["soluong"].ToString());
                                ctpttnxb.purchase_payment_item_price = decimal.Parse(db.books.Find(ctpttnxb.fk_book).book_purchase_price.ToString());
                                ctpttnxb.purchase_payment_item_money = ctpttnxb.purchase_payment_item_quantity * ctpttnxb.purchase_payment_item_price;

                                if(ctpttnxb.purchase_payment_item_quantity <= 0)
                                {
                                    ViewBag.loi = "Số lượng thanh toán phải lớn hơn 0";
                                    goto baoloi;
                                }

                                // Kiểm tra xem có thanh toán hơn nợ mà đang thiếu không
                                int? checksoluong = db.supplier_debt_item.FirstOrDefault(m => m.fk_supplier_debt == d.supplier_debt_id && m.fk_book == ctpttnxb.fk_book).supplier_debt_item_quantity;
                                if(ctpttnxb.purchase_payment_item_quantity <= checksoluong)
                                {
                                    ((List<purchase_payment_item>)Session["ctPhieuThanhToanNXB"]).Add(ctpttnxb);
                                }
                                else
                                {
                                    ViewBag.loi = "Vượt quá số lượng nợ hiện tại, số lượng nợ hiện tại là: " + checksoluong;
                                    goto baoloi;
                                }     
                        }
                        return RedirectToAction("ThemChiTietPhieuThanhToanNXB");
                    }
                }
            }


            else if (Request.Form["create"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (((List<purchase_payment_item>)Session["ctPhieuThanhToanNXB"]).Count == 0)
                    {
                        ViewBag.loi = "Không được để phiếu trống";
                        goto baoloi;
                    }

                    var luu = db.purchase_payment.Add((purchase_payment)Session["PhieuThanhToanNXB"]);    // Lưu phiếu thanh toán ( chưa ghi tổng tiền phiếu )
                    db.SaveChanges();

                    decimal tongTien = 0;
                    decimal? temptongtien = 0;

                    foreach (var ctpttnxb in (List<purchase_payment_item>)Session["ctPhieuThanhToanNXB"])
                    {
                        temptongtien += decimal.Parse((ctpttnxb.purchase_payment_item_price * ctpttnxb.purchase_payment_item_quantity).ToString());
                        tongTien = decimal.Parse(temptongtien.ToString());

                        //lưu chi tiết phiếu xuất
                        purchase_payment_item ctPhieuThanhToanNXB = new purchase_payment_item();
                        ctPhieuThanhToanNXB = ctpttnxb;
                        ctPhieuThanhToanNXB.book = null;
                        db.purchase_payment_item.Add(ctPhieuThanhToanNXB);           
                    }

                    //lưu phiếu xuất
                    supplier_debt debt1 = db.supplier_debt.OrderByDescending(m => m.supplier_debt_id).FirstOrDefault(m => m.fk_supplier == (int)luu.fk_supplier);
                    if (debt1 == null || (debt1 != null && debt1.supplier_debt_total_money > 0 && debt1.supplier_debt_total_money >= tongTien) || debt1.supplier_debt_total_money == 0)
                    {
                        luu.purchase_payment_money_pay = tongTien;
                        db.purchase_payment.Attach(luu);
                        db.Entry(luu).State = EntityState.Modified;
                        db.SaveChanges();

                        //cập nhật công nợ Đại Lý
                        supplier_debt debt = new supplier_debt();
                        debt.supplier_debt_time = DateTime.Now;
                        debt.fk_supplier = luu.fk_supplier;
                        debt.supplier_debt_total_money = debt1.supplier_debt_total_money - luu.purchase_payment_money_pay;
                        
                        db.supplier_debt.Add(debt);

                        //cập nhật chi tiết công nợ Đại lý
                        int ad_id = (db.supplier_debt.Max(u => u.supplier_debt_id)) + 1;

                        supplier_debt ad = db.supplier_debt.OrderByDescending(a => a.supplier_debt_time).FirstOrDefault(a => a.fk_supplier == debt.fk_supplier);
                        List<supplier_debt_item> list = ad.supplier_debt_item.ToList();
                        foreach (var ctno in list)
                        {
                            int check = 1;
                            foreach (var ctpttnxb in (List<purchase_payment_item>)Session["ctPhieuThanhToanNXB"])
                            {
                                if (ctno.fk_book == ctpttnxb.fk_book)
                                {
                                    check = 0;
                                    break;
                                }
                            }
                            if (check == 1)
                            {
                                supplier_debt_item adi = ctno;
                                adi.fk_supplier_debt = ad_id;
                                db.supplier_debt_item.Add(adi);
                            }
                        }
                        

                        foreach (var ctpttnxb in (List<purchase_payment_item>)Session["ctPhieuThanhToanNXB"])
                        {
                            supplier_debt_item adi = new supplier_debt_item();
                            adi.fk_supplier_debt = ad_id;
                            adi.fk_book = ctpttnxb.fk_book;  
                            adi.supplier_debt_item_quantity = ad.supplier_debt_item.FirstOrDefault(a => a.fk_book == ctpttnxb.fk_book).supplier_debt_item_quantity - ctpttnxb.purchase_payment_item_quantity;
                            adi.supplier_debt_item_money = ad.supplier_debt_item.FirstOrDefault(a => a.fk_book == ctpttnxb.fk_book).supplier_debt_item_money - ctpttnxb.purchase_payment_item_money;
                            if(adi.supplier_debt_item_quantity > 0 && adi.supplier_debt_item_money > 0)
                            {
                                db.supplier_debt_item.Add(adi);
                            }
                            db.SaveChanges();
                        }
                        Session["ctPhieuThanhToanNXB"] = null;
                        Session["PhieuThanhToanNXB"] = null;
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
            purchase_payment purchase_payment = db.purchase_payment.Find(id);
            if (purchase_payment == null)
            {
                return HttpNotFound();
            }
            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name", purchase_payment.fk_supplier);
            return View(purchase_payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "purchase_payment_id,fk_supplier,purchase_payment_time,purchase_payment_money_pay")] purchase_payment purchase_payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchase_payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name", purchase_payment.fk_supplier);
            return View(purchase_payment);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            purchase_payment purchase_payment = db.purchase_payment.Find(id);
            if (purchase_payment == null)
            {
                return HttpNotFound();
            }
            return View(purchase_payment);
        }

        // POST: /purchase_payment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            purchase_payment purchase_payment = db.purchase_payment.Find(id);
            db.purchase_payment.Remove(purchase_payment);
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
