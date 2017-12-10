namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("store")]
    public partial class store
    {
        [Key]

        [Display(Name = "Mã tồn kho")]
        public int store_id { get; set; }

        [Display(Name = "Mã sách")]
        public int fk_book { get; set; }

        [Display(Name = "Số lượng tồn")]
        public int? store_quantity { get; set; }

        [Display(Name = "Giá bán")]
        public decimal? store_seling_price { get; set; }

        [Display(Name = "Giá nhập")]
        public decimal? store_purchase_price { get; set; }

        [Display(Name = "Thời điểm tồn")]
        public DateTime? store_time { get; set; }

        public virtual book book { get; set; }
    }
}
