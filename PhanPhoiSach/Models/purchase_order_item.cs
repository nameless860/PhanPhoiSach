namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class purchase_order_item
    {
        [Key]

        [Display(Name = "Mã CT phiếu nhập")]
        public int purchase_order_item_id { get; set; }

        [Display(Name = "Mã phiếu nhập")]
        public int fk_purchase_order { get; set; }

        [Display(Name = "Mã sách")]
        public int fk_book { get; set; }

        [Display(Name = "Giá nhập")]
        public decimal? purchase_order_item_price { get; set; }

        [Display(Name = "Số lượng")]
        public int? purchase_order_item_quantity { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal? purchase_order_money { get; set; }

        public virtual book book { get; set; }

        public virtual purchase_order purchase_order { get; set; }
    }
}
