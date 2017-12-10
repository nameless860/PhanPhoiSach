namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class purchase_payment_item
    {
        [Key]
        public int purchase_payment_item_id { get; set; }

        public int fk_purchase_payment { get; set; }

        public int fk_book { get; set; }

        public int? purchase_payment_item_quantity { get; set; }

        public decimal? purchase_payment_item_price { get; set; }

        public decimal? purchase_payment_item_money { get; set; }

        public virtual book book { get; set; }

        public virtual purchase_payment purchase_payment { get; set; }
    }
}
