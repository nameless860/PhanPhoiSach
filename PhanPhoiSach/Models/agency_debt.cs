namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class agency_debt
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public agency_debt()
        {
            agency_debt_item = new HashSet<agency_debt_item>();
        }

        [Key]
        [Display(Name = "Mã nợ đại lý")]
        public int agency_debt_id { get; set; }

        [Display(Name = "Mã đại lý")]
        public int fk_agency { get; set; }

        [Display(Name = "Tổng tiền nợ")]
        public decimal? agency_debt_total_money { get; set; }

        [Display(Name = "Thời điểm nợ")]
        public DateTime? agency_debt_time { get; set; }

        public virtual agency agency { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<agency_debt_item> agency_debt_item { get; set; }
    }
}
