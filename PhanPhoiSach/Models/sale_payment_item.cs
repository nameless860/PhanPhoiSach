namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class sale_payment_item
    {
        [Key]
        public int sale_payment_item_id { get; set; }

        public int fk_sale_payment { get; set; }

        public int fk_book { get; set; }

        public int? sale_payment_item_quantity { get; set; }

        public decimal? sale_payment_item_price { get; set; }

        public decimal? sale_payment_item_money { get; set; }

        public virtual book book { get; set; }

        public virtual sale_payment sale_payment { get; set; }
    }
}
