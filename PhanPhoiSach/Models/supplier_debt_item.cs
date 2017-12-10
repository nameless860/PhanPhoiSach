namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class supplier_debt_item
    {
        [Key]

        [Display(Name = "Mã CT nợ")]
        public int supplier_debt_item_id { get; set; }

        [Display(Name = "Mã phiếu nợ")]
        public int fk_supplier_debt { get; set; }

        [Display(Name = "Mã sách")]
        public int fk_book { get; set; }

        [Display(Name = "Số lượng")]
        public int? supplier_debt_item_quantity { get; set; }

        [Display(Name = "Tổng tiền nợ")]
        public decimal? supplier_debt_item_money { get; set; }

        public virtual book book { get; set; }

        public virtual supplier_debt supplier_debt { get; set; }
    }
}
