namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class purchase_payment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public purchase_payment()
        {
            purchase_payment_item = new HashSet<purchase_payment_item>();
        }

        [Key]
        public int purchase_payment_id { get; set; }

        public int fk_supplier { get; set; }

        [Display(Name = "Số tiền thanh toán")]
        public decimal? purchase_payment_money_pay { get; set; }

        [Display(Name = "Ngày thanh toán")]
        public DateTime? purchase_payment_time { get; set; }

        public virtual supplier supplier { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<purchase_payment_item> purchase_payment_item { get; set; }
    }
}
