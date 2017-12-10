namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class agency_debt_item
    {
        [Key]

        [Display(Name = "Mã chi tiết nợ")]
        public int agency_debt_item_id { get; set; }

        [Display(Name = "Mã phiếu nợ")]
        public int fk_agency_debt { get; set; }

        [Display(Name = "Mã sách")]
        public int fk_book { get; set; }

        [Display(Name = "Số lượng nợ")]
        public int? agency_debt_item_quantity { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal? agency_debt_item_money { get; set; }

        public virtual agency_debt agency_debt { get; set; }

        public virtual book book { get; set; }
    }
}
