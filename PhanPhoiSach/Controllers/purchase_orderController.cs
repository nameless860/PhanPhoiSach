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
    public class purchase_orderController : Controller
    {
        private Model1 db = new Model1();

        public ActionResult Index()
        {
            var purchase_order = db.purchase_order.Include(p => p.supplier); // load tất cả phiếu lên View
            return View(purchase_order.ToList());
        }


        public ActionResult Deletectpn(int bookid)
        {
            var list = (List<purchase_order_item>)Session["ctphieunhap"];
            list.RemoveAll(p => p.fk_book == bookid);
            return RedirectToAction("ThemChiTietPhieuNhap");
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            purchase_order purchase_order = db.purchase_order.Find(id); // Tìm phiếu theo ID trong CSDL
            if (purchase_order == null)
            {
                return HttpNotFound();
            }
            return View(purchase_order);
        }

        // GET: /purchase_order/Create
        public ActionResult Create()
        {
            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name"); // Load danh sách các NXB lên Select List trên View
            return View();
        }

        [HttpPost] // giao thức
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="purchase_order_id,fk_supplier,purchase_order_recipent,purchase_order_created_at,purchase_order_total_money")] purchase_order purchase_order)
        {
            if (ModelState.IsValid)
            {
                purchase_order.purchase_order_created_at = DateTime.Now;     
                Session["PhieuNhap"] = purchase_order; //  Phiên làm việc cho phiếu xuất
                Session["ctphieunhap"] = new List<purchase_order_item>(); // Tạo 1 phiên làm việc cho CHI TIẾT phiếu xuất
                return RedirectToAction("ThemChiTietPhieuNhap");
            }

            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name", purchase_order.fk_supplier);
            return View(purchase_order);
        }

        [HttpGet]
        public ActionResult ThemChiTietPhieuNhap()
        {
            // Load danh sách các SÁCH(tùy theo sách của NXB nào) lên Select List trên View
            purchase_order po = (purchase_order)Session["PhieuNhap"];
            var books = ( from b in db.books
                          where b.fk_supplier == po.fk_supplier
                          select b );
            ViewBag.sach = new SelectList(books, "book_id", "book_name");
            return View();
        }

        public ActionResult ThemChiTietPhieuNhap(FormCollection chitiet)
        {
            purchase_order po = (purchase_order)Session["PhieuNhap"];
            var books = (from b in db.books
                         where b.fk_supplier == po.fk_supplier
                         select b);
            ViewBag.sach = new SelectList(books, "Book_id", "Book_name");
            ViewBag.loi = null;

            if (Request.Form["add"] != null)  // Nếu Click nút THÊM trên View ( thêm một sách mới vào CT phiếu nhập )
            {
                if (ModelState.IsValid)
                {
                    bool check = true;
                    // Kiểm tra xem SÁCH này đã được thêm trước đó trong chi tiết phiếu nhập chưa
                    foreach (var ctpn in (List<purchase_order_item>)Session["ctphieunhap"])
                    {
                        if (ctpn.fk_book == Int32.Parse(chitiet["sach"].ToString()))
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
                            ViewBag.loi = "Không tồn tại sách để nhập";
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
                            // Thêm chi tiết mới vào Session["ctphieuxuat"]
                            purchase_order_item ctpn = new purchase_order_item();
                            ctpn.fk_purchase_order = (db.purchase_order.Max(u => (int?)u.purchase_order_id) != null ? db.purchase_order.Max(u => u.purchase_order_id) : 0) + 1;
                            ctpn.fk_book = Int32.Parse(chitiet["sach"].ToString());
                            ctpn.book = db.books.Find(Int32.Parse(chitiet["sach"].ToString()));
                            ctpn.purchase_order_item_quantity = Int32.Parse(chitiet["soluong"].ToString());
                            ctpn.purchase_order_item_price = decimal.Parse(db.books.Find(ctpn.fk_book).book_purchase_price.ToString());
                            ctpn.purchase_order_money = ctpn.purchase_order_item_quantity * ctpn.purchase_order_item_price;

                            if (ctpn.purchase_order_item_quantity <= 0)
                            {
                                ViewBag.loi = "Số lượng nhập phải lớn hơn 0";
                                goto baoloi;
                            }

                            //kiểm tra tổng tiền của phiếu có lớn hơn số nợ hay không
                            //khai báo để tìm nợ của NXB đó
                            purchase_order phieunhap = (purchase_order)Session["PhieuNhap"];
                            supplier_debt noNXB = db.supplier_debt.OrderByDescending(m => m.supplier_debt_id).FirstOrDefault(m => m.fk_supplier == (int)phieunhap.fk_supplier);
                            
                            decimal checktien = 0;
                            foreach (var checktest in (List<purchase_order_item>)Session["ctphieunhap"])  // Cộng tổng tiền tất cả các chi tiết
                            {
                                checktien = checktien + decimal.Parse(checktest.purchase_order_money.ToString());
                            }
                            checktien += decimal.Parse(ctpn.purchase_order_money.ToString());

                            if (noNXB == null || (noNXB != null && noNXB.supplier_debt_total_money > 0 && noNXB.supplier_debt_total_money >= checktien) || noNXB.supplier_debt_total_money == 0)
                            {
                                ((List<purchase_order_item>)Session["ctphieunhap"]).Add(ctpn);
                            }
                            else
                            {
                                ViewBag.loi = "Vượt quá số nợ cho phép, mức nợ hiện tại là: " + noNXB.supplier_debt_total_money;
                                goto baoloi;
                            }
                        }
                        return RedirectToAction("ThemChiTietPhieuNhap");
                    }
                }
            }


            else if (Request.Form["create"] != null)  // Nếu Click nút LƯU PHIẾU trên View
            {
                if (ModelState.IsValid)
                {
                    if (((List<purchase_order_item>)Session["ctphieunhap"]).Count == 0)
                    {
                        ViewBag.loi = "Không được để phiếu trống";
                        goto baoloi;
                    }
                    
                    var luu = db.purchase_order.Add((purchase_order)Session["PhieuNhap"]);  // Lưu phiếu xuất ( chưa ghi tổng tiền phiếu )
                    db.SaveChanges();

                    decimal tongTien = 0;
                    decimal? temptongtien = 0;

                    foreach (var ctpn in (List<purchase_order_item>)Session["ctphieunhap"])
                    {
                        temptongtien += decimal.Parse((ctpn.purchase_order_item_price * ctpn.purchase_order_item_quantity).ToString());
                        tongTien = decimal.Parse(temptongtien.ToString());

                        //lưu chi tiết phiếu nhập
                        purchase_order_item ctphieunhap = new purchase_order_item();
                        ctphieunhap = ctpn;
                        ctphieunhap.book = null;
                        db.purchase_order_item.Add(ctphieunhap);

                        //cập nhật STORE
                        store tonkho = new store();
                        tonkho.store_time = DateTime.Now;
                        tonkho.fk_book = ctpn.fk_book;

                        store tonkhohientai = db.stores.OrderByDescending(m => m.store_id).FirstOrDefault(m => m.fk_book == (int)ctpn.fk_book);
                        if(tonkhohientai != null)
                        {
                            tonkho.store_quantity = tonkhohientai.store_quantity + ctpn.purchase_order_item_quantity;
                        }
                        else
                        {
                            tonkho.store_quantity = ctpn.purchase_order_item_quantity;
                        }

                        // Cập nhật thuộc tính book_stock trong BOOK
                        book book = db.books.Find(Int32.Parse(ctpn.fk_book.ToString()));
                        book.book_stock = tonkho.store_quantity;
                        db.books.Attach(book);
                        db.Entry(book).State = EntityState.Modified;

                        tonkho.store_seling_price = book.book_seling_price;
                        tonkho.store_purchase_price = book.book_purchase_price;

                        db.stores.Add(tonkho);

                    }

                    //lưu phiếu nhập
                    supplier_debt debt1 = db.supplier_debt.OrderByDescending(m => m.supplier_debt_id).FirstOrDefault(m => m.fk_supplier == (int)luu.fk_supplier);
                    if (debt1 == null || (debt1 != null && debt1.supplier_debt_total_money > 0 && debt1.supplier_debt_total_money >= tongTien) || debt1.supplier_debt_total_money == 0)
                    {
                        // cập nhật tổng tiền phiếu cho phiếu nhập
                        luu.purchase_order_total_money = tongTien;
                        db.purchase_order.Attach(luu);
                        db.Entry(luu).State = EntityState.Modified;
                        db.SaveChanges();

                        //cập nhật NỢ NXB
                        supplier_debt debt = new supplier_debt();
                        debt.supplier_debt_time = DateTime.Now;
                        if (debt1 != null)
                        {
                            debt.fk_supplier = luu.fk_supplier;
                            debt.supplier_debt_total_money = debt1.supplier_debt_total_money + luu.purchase_order_total_money;
                        }
                        else
                        {
                            debt.fk_supplier = luu.fk_supplier;
                            debt.supplier_debt_total_money = luu.purchase_order_total_money;
                        }

                        db.supplier_debt.Add(debt);

                        //cập nhật CHI TIẾT NỢ NXB: 

                        //// Thêm các chi tiết đã nợ trước đó nhưng không có trong Session["ctphieunhap"]
                        int sd_id = (db.supplier_debt.Max(u => (int?)u.supplier_debt_id) != null ? db.supplier_debt.Max(u => u.supplier_debt_id) :0) + 1 ;  // supplier_debt id          
                        supplier_debt sd = db.supplier_debt.OrderByDescending(a => a.supplier_debt_time).FirstOrDefault(a => a.fk_supplier == debt.fk_supplier);    
                        if (sd != null) 
                        {
                            if(sd.supplier_debt_total_money > 0)
                            {
                                List<supplier_debt_item> list = sd.supplier_debt_item.ToList();
                                foreach (var ctpn in list)
                                {
                                    int check = 1;
                                    foreach (var ctpn2 in (List<purchase_order_item>)Session["ctphieunhap"])
                                    {
                                        if (ctpn.fk_book == ctpn2.fk_book || ctpn.supplier_debt_item_quantity == 0)
                                        {
                                            check = 0;
                                            break;
                                        }
                                    }
                                    if (check == 1)
                                    {
                                        supplier_debt_item sdi = ctpn;
                                        sdi.fk_supplier_debt = sd_id;
                                        db.supplier_debt_item.Add(sdi);
                                    }
                                }
                            }
                        }

                        //// Thêm các chi tiết nợ mới trong Session["ctphieuxuat"])
                        foreach (var ctpn in (List<purchase_order_item>)Session["ctphieunhap"])
                        {
                            supplier_debt_item sdi = new supplier_debt_item();

                            sdi.fk_supplier_debt = sd_id;

                            sdi.fk_book = ctpn.fk_book;

                            if (sd != null)  //  Đã có nợ từ trước
                            {
                                if (sd.supplier_debt_item.FirstOrDefault(a => a.fk_book == ctpn.fk_book) != null)  // Đã nợ sách này từ trước
                                {
                                    sdi.supplier_debt_item_quantity = sd.supplier_debt_item.FirstOrDefault(a => a.fk_book == ctpn.fk_book).supplier_debt_item_quantity + ctpn.purchase_order_item_quantity;
                                    sdi.supplier_debt_item_money = sd.supplier_debt_item.FirstOrDefault(a => a.fk_book == ctpn.fk_book).supplier_debt_item_money + ctpn.purchase_order_money;
                                }
                                else
                                {
                                    sdi.supplier_debt_item_quantity = ctpn.purchase_order_item_quantity;
                                    sdi.supplier_debt_item_money = ctpn.purchase_order_money;
                                }

                            }
                            else
                            {
                                sdi.supplier_debt_item_quantity = ctpn.purchase_order_item_quantity;
                                sdi.supplier_debt_item_money = ctpn.purchase_order_money;
                            }

                            db.supplier_debt_item.Add(sdi);
                            db.SaveChanges();
                        }

                        Session["ctphieunhap"] = null;
                        Session["PhieuNhap"] = null;
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

        // GET: /purchase_order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            purchase_order purchase_order = db.purchase_order.Find(id);
            if (purchase_order == null)
            {
                return HttpNotFound();
            }
            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name", purchase_order.fk_supplier);
            return View(purchase_order);
        }

        // POST: /purchase_order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="purchase_order_id,fk_supplier,purchase_order_recipent,purchase_order_created_at,purchase_order_total_money")] purchase_order purchase_order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchase_order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name", purchase_order.fk_supplier);
            return View(purchase_order);
        }

        // GET: /purchase_order/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            purchase_order purchase_order = db.purchase_order.Find(id);
            if (purchase_order == null)
            {
                return HttpNotFound();
            }
            return View(purchase_order);
        }

        // POST: /purchase_order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            purchase_order purchase_order = db.purchase_order.Find(id);
            db.purchase_order.Remove(purchase_order);
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
