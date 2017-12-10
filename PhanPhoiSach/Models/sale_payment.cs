namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class sale_payment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sale_payment()
        {
            sale_payment_item = new HashSet<sale_payment_item>();
        }

        [Key]
        public int sale_payment_id { get; set; }

        public int fk_agency { get; set; }

        [Display(Name = "Số tiền thanh toán")]
        public decimal? sale_payment_money_pay { get; set; }

        [Display(Name = "Ngày thanh toán")]
        public DateTime? sale_payment_time { get; set; }

        public virtual agency agency { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sale_payment_item> sale_payment_item { get; set; }
    }
}
