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
    public class sale_orderController : Controller
    {
        private Model1 db = new Model1();

        public ActionResult Index()
        {
            var sale_order = db.sale_order.Include(s => s.agency); // load tất cả phiếu xuất lên View
            return View(sale_order.ToList());
        }

        public ActionResult Deletectpx(int bookid)
        {
            var list = (List<sale_order_item>)Session["ctphieuxuat"];
            list.RemoveAll(p => p.fk_book == bookid);
            return RedirectToAction("ThemChiTietPhieuXuat");
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            sale_order sale_order = db.sale_order.Find(id); // Tìm phiếu xuất theo ID trong CSDL
            if (sale_order == null)
            {
                return HttpNotFound();
            }
            return View(sale_order);
        }

        public ActionResult Create()
        {
            ViewBag.fk_agency = new SelectList(db.agencies,"agency_id", "agency_name");  // Load danh sách các Đại lý lên Select List trên View
            return View();
        }

        [HttpPost]  // giao thức
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="sale_order_id,fk_agency,sale_order_recipent,sale_order_created_at,sale_order_total_money")] sale_order sale_order)
        {
            if (ModelState.IsValid)
            {
                sale_order.sale_order_created_at = DateTime.Now;
                Session["PhieuXuat"] = sale_order;  //  Phiên làm việc cho phiếu xuất
                Session["ctphieuxuat"] = new List<sale_order_item>(); // Tạo 1 phiên làm việc cho CHI TIẾT phiếu xuất
                return RedirectToAction("ThemChiTietPhieuXuat");
            }

            ViewBag.fk_agency = new SelectList(db.agencies, "agency_id", "agency_name", sale_order.fk_agency);
            return View(sale_order);
        }

        [HttpGet]
        public ActionResult ThemChiTietPhieuXuat()
        {
            ViewBag.sach = new SelectList(db.books, "book_id", "book_name");  // Load danh sách các SÁCH lên Select List trên View
            return View();
        }

        public ActionResult ThemChiTietPhieuXuat(FormCollection chitiet)
        {
            ViewBag.sach = new SelectList(db.books, "Book_id", "Book_name");
            ViewBag.loi = null;

            if (Request.Form["add"] != null) // Nếu Click nút THÊM trên View ( thêm một sách mới vào CT phiếu xuất )
            {
                if (ModelState.IsValid)
                {
                    bool check = true;
                    // Kiểm tra xem SÁCH này đã được thêm trước đó trong chi tiết phiếu xuất chưa
                    foreach (var ctpx in (List<sale_order_item>)Session["ctphieuxuat"])
                    {
                        if (ctpx.fk_book == Int32.Parse(chitiet["sach"].ToString()))
                        {
                            check = false;
                            ViewBag.loi = "Sách đã được thêm vào phiếu trước đó";
                            break;
                        }
                    }

                    // Nếu sách chưa được thêm thì kiểm tra tiếp như sau:
                    if (check)
                    {
                        //kiểm tra xem sách này có tồn tại hay không
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
                            //kiểm tra tồn kho
                            store tonkho_sach = db.stores.OrderByDescending(m => m.store_id).FirstOrDefault(m => m.fk_book == (int)sach.book_id);
                            if (tonkho_sach == null)
                            {
                                ViewBag.loi = "Không có tồn kho";
                                goto baoloi;
                            }
                            else
                            {
                                //nếu tồn kho == 0
                                if (tonkho_sach.store_quantity == 0)
                                {
                                    ViewBag.loi = "Sách đã hết trong kho";
                                    goto baoloi;
                                }
                                else
                                {
                                    //kiểm tra điều kiện vượt quá số lượng trong kho
                                    int soluong = Int32.Parse(chitiet["soluong"].ToString());
                                    if (tonkho_sach.store_quantity < soluong)
                                    {
                                        ViewBag.loi = "Số lượng sách không đủ để xuất, số lượng hiện có là " + tonkho_sach.store_quantity + " cuốn";
                                        goto baoloi;
                                    }
                                    else
                                    {
                                        // Thêm chi tiết mới vào Session["ctphieuxuat"]
                                        sale_order_item ctpx = new sale_order_item();
                                        ctpx.fk_sale_order = (db.sale_order.Max(u => (int?)u.sale_order_id) != null ? db.sale_order.Max(u => u.sale_order_id) : 0) + 1;
                                        ctpx.fk_book = Int32.Parse(chitiet["sach"].ToString());
                                        ctpx.book = db.books.Find(Int32.Parse(chitiet["sach"].ToString()));
                                        ctpx.sale_order_item_quantity = Int32.Parse(chitiet["soluong"].ToString());
                                        ctpx.sale_order_item_price = decimal.Parse(db.books.Find(ctpx.fk_book).book_seling_price.ToString());
                                        ctpx.sale_order_money = ctpx.sale_order_item_quantity * ctpx.sale_order_item_price;

                                        if (ctpx.sale_order_item_quantity <= 0)
                                        {
                                            ViewBag.loi = "Số lượng xuất phải lớn hơn 0";
                                            goto baoloi;
                                        }

                                        //kiểm tra tổng tiền của phiếu có lớn hơn số nợ của đại lý hay không
                                        //khai báo để tìm nợ của đại lý đó
                                        sale_order phieuxuat = (sale_order)Session["PhieuXuat"];   // Lấy phiếu xuất đang tạo
                                        agency_debt noDL = db.agency_debt.OrderByDescending(m => m.agency_debt_id).FirstOrDefault(m => m.fk_agency == (int)phieuxuat.fk_agency);  //  Tìm NỢ mới nhất của đại lý này

                                        decimal checktien = 0;
                                        foreach (var checktest in (List<sale_order_item>)Session["ctphieuxuat"])  // Cộng tổng tiền tất cả các chi tiết
                                        {
                                            checktien = checktien + decimal.Parse(checktest.sale_order_money.ToString());
                                        }
                                        checktien += decimal.Parse(ctpx.sale_order_money.ToString());

                                        if (noDL == null || (noDL != null && noDL.agency_debt_total_money > 0 && noDL.agency_debt_total_money >= checktien) || noDL.agency_debt_total_money == 0)
                                        {
                                            ((List<sale_order_item>)Session["ctphieuxuat"]).Add(ctpx);
                                        }
                                        else
                                        {
                                            ViewBag.loi = "Vượt quá số nợ cho phép, mức nợ hiện tại là: " + noDL.agency_debt_total_money;
                                            goto baoloi;
                                        }
                                    }
                                }
                            }
                        }
                        return RedirectToAction("ThemChiTietPhieuXuat");
                    }
                }
            }

            else if (Request.Form["create"] != null)  // Nếu Click nút LƯU PHIẾU trên View
            {
                if (ModelState.IsValid)
                {
                    if (((List<sale_order_item>)Session["ctphieuxuat"]).Count == 0)
                    {
                        ViewBag.loi = "Không được để phiếu trống";
                        goto baoloi;
                    }

                    var luu = db.sale_order.Add((sale_order)Session["PhieuXuat"]);  // Lưu phiếu xuất ( chưa ghi tổng tiền phiếu )
                    db.SaveChanges();

                    decimal tongTien = 0;
                    decimal? temptongtien = 0;

                    foreach (var ctpx in (List<sale_order_item>)Session["ctphieuxuat"])
                    {
                        temptongtien += decimal.Parse((ctpx.sale_order_item_price * ctpx.sale_order_item_quantity).ToString());
                        tongTien = decimal.Parse(temptongtien.ToString());

                        //lưu chi tiết phiếu xuất
                        sale_order_item ctPhieuXuat = new sale_order_item();
                        ctPhieuXuat = ctpx;
                        ctPhieuXuat.book = null;
                        db.sale_order_item.Add(ctPhieuXuat);

                        //cập nhật STORE
                        store tonkho = new store();
                        tonkho.store_time = DateTime.Now;
                        tonkho.fk_book = ctpx.fk_book;
                        store tonkhohientai = db.stores.OrderByDescending(m => m.store_id).FirstOrDefault(m => m.fk_book == (int)ctpx.fk_book);
                        tonkho.store_quantity= tonkhohientai.store_quantity - ctpx.sale_order_item_quantity;

                        // Cập nhật thuộc tính book_stock trong BOOK
                        book book = db.books.Find(Int32.Parse(ctpx.fk_book.ToString()));
                        book.book_stock = tonkho.store_quantity;
                        db.books.Attach(book);
                        db.Entry(book).State = EntityState.Modified;

                        tonkho.store_seling_price = book.book_seling_price;
                        tonkho.store_purchase_price = book.book_purchase_price;

                        db.stores.Add(tonkho);
                    }

                    //lưu phiếu xuất
                    agency_debt debt1 = db.agency_debt.OrderByDescending(m => m.agency_debt_id).FirstOrDefault(m => m.fk_agency == (int)luu.fk_agency);
                    if (debt1 == null || (debt1 != null && debt1.agency_debt_total_money > 0 && debt1.agency_debt_total_money >= tongTien) || debt1.agency_debt_total_money == 0)
                    {
                        // cập nhật tổng tiền phiếu cho phiếu xuất
                        luu.sale_order_total_money = tongTien;
                        db.sale_order.Attach(luu);
                        db.Entry(luu).State = EntityState.Modified;
                        db.SaveChanges();

                        //cập nhật NỢ Đại Lý
                        agency_debt debt = new agency_debt();
                        debt.agency_debt_time = DateTime.Now;
                        if (debt1 != null)
                        {
                            debt.fk_agency = luu.fk_agency;
                            debt.agency_debt_total_money = debt1.agency_debt_total_money + luu.sale_order_total_money;
                        }
                        else
                        {
                            debt.fk_agency = luu.fk_agency;
                            debt.agency_debt_total_money = luu.sale_order_total_money;
                        }

                        db.agency_debt.Add(debt);

                        //cập nhật CHI TIẾT NỢ đại lý:

                        //// Thêm các chi tiết đã nợ trước đó nhưng không có trong Session["ctphieuxuat"]
                        int ad_id = (db.agency_debt.Max(u => (int?)u.agency_debt_id) != null ? db.agency_debt.Max(u => u.agency_debt_id) :0) + 1 ;  // agency_debt id           
                        agency_debt ad = db.agency_debt.OrderByDescending(a => a.agency_debt_time).FirstOrDefault(a => a.fk_agency == debt.fk_agency);
                        if (ad != null)
                        {
                            if(ad.agency_debt_total_money > 0)
                            {
                                List<agency_debt_item> list = ad.agency_debt_item.ToList();
                                foreach (var ctno in list)
                                {
                                    int check = 1;
                                    foreach (var ctpx in (List<sale_order_item>)Session["ctphieuxuat"])
                                    {
                                        if (ctno.fk_book == ctpx.fk_book || ctno.agency_debt_item_quantity == 0)
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
                            }                        
                        }

                        //// Thêm các chi tiết nợ mới trong Session["ctphieuxuat"])
                        foreach (var ctpx in (List<sale_order_item>)Session["ctphieuxuat"])
                        {
                            agency_debt_item adi = new agency_debt_item();

                            adi.fk_agency_debt = ad_id;

                            adi.fk_book = ctpx.fk_book;

                            if(ad != null)  //  Đã có nợ từ trước
                            {
                                if (ad.agency_debt_item.FirstOrDefault(a => a.fk_book == ctpx.fk_book) != null)  // Đã nợ sách này từ trước
                                {
                                    adi.agency_debt_item_quantity = ad.agency_debt_item.FirstOrDefault(a => a.fk_book == ctpx.fk_book).agency_debt_item_quantity + ctpx.sale_order_item_quantity;
                                    adi.agency_debt_item_money = ad.agency_debt_item.FirstOrDefault(a => a.fk_book == ctpx.fk_book).agency_debt_item_money + ctpx.sale_order_money;
                                }
                                else
                                {
                                    adi.agency_debt_item_quantity = ctpx.sale_order_item_quantity;
                                    adi.agency_debt_item_money = ctpx.sale_order_money;
                                }
                            }
                            else
                            {
                                adi.agency_debt_item_quantity = ctpx.sale_order_item_quantity;
                                adi.agency_debt_item_money = ctpx.sale_order_money;
                            }
                            db.agency_debt_item.Add(adi);
                            db.SaveChanges();
                        }
                        
                        Session["ctphieuxuat"] = null;
                        Session["PhieuXuat"] = null;
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

        // GET: /sale_order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sale_order sale_order = db.sale_order.Find(id);
            if (sale_order == null)
            {
                return HttpNotFound();
            }
            ViewBag.fk_agency = new SelectList(db.agencies, "agency_id", "agency_name", sale_order.fk_agency);
            return View(sale_order);
        }

        // POST: /sale_order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="sale_order_id,fk_agency,sale_order_recipent,sale_order_created_at,sale_order_total_money")] sale_order sale_order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sale_order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.fk_agency = new SelectList(db.agencies, "agency_id", "agency_name", sale_order.fk_agency);
            return View(sale_order);
        }

        // GET: /sale_order/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sale_order sale_order = db.sale_order.Find(id);
            if (sale_order == null)
            {
                return HttpNotFound();
            }
            return View(sale_order);
        }

        // POST: /sale_order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            sale_order sale_order = db.sale_order.Find(id);
            db.sale_order.Remove(sale_order);
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
