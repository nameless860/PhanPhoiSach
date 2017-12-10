namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class purchase_order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public purchase_order()
        {
            purchase_order_item = new HashSet<purchase_order_item>();
        }

        [Key]
        [Display(Name = "Mã phiếu nhập")]
        public int purchase_order_id { get; set; }

        [Display(Name = "Mã NXB")]
        public int fk_supplier { get; set; }

        [StringLength(50)]
        [Display(Name = "Người giao sách")]
        public string purchase_order_recipent { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? purchase_order_created_at { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal? purchase_order_total_money { get; set; }

        public virtual supplier supplier { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<purchase_order_item> purchase_order_item { get; set; }
    }
}
