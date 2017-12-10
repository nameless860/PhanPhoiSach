namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class sale_order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sale_order()
        {
            sale_order_item = new HashSet<sale_order_item>();
        }

        [Key]

        [Display(Name="Mã phiếu xuất")]
        public int sale_order_id { get; set; }

        [Display(Name = "Mã đại lý")]
        public int fk_agency { get; set; }

        [StringLength(50)]
        [Display(Name = "Người nhận sách")]
        public string sale_order_recipent { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? sale_order_created_at { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal? sale_order_total_money { get; set; }

        public virtual agency agency { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sale_order_item> sale_order_item { get; set; }
    }
}
