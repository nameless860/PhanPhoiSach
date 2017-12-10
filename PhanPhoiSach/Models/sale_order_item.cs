namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class sale_order_item
    {
        [Key]

        [Display(Name = "Mã CT Xuất")]
        public int sale_order_item_id { get; set; }

        [Display(Name = "Mã phiếu xuất")]
        public int fk_sale_order { get; set; }

        [Display(Name = "Mã sách")]
        public int fk_book { get; set; }

        [Display(Name = "Giá xuất")]
        public decimal? sale_order_item_price { get; set; }

        [Display(Name = "Số lượng")]
        public int? sale_order_item_quantity { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal? sale_order_money { get; set; }

        public virtual book book { get; set; }

        public virtual sale_order sale_order { get; set; }
    }
}
