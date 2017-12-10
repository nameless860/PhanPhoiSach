namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("supplier")]
    public partial class supplier
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public supplier()
        {
            books = new HashSet<book>();
            purchase_order = new HashSet<purchase_order>();
            supplier_debt = new HashSet<supplier_debt>();
            purchase_payment = new HashSet<purchase_payment>();
        }

        [Key]
        public int supplier_id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên NXB")]
        public string supplier_name { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Địa chỉ")]
        public string supplier_address { get; set; }

        [StringLength(12)]
        [Display(Name = "SĐT")]
        public string supplier_phone { get; set; }

        [StringLength(12)]
        [Display(Name = "Số tài khoản")]
        public string supplier_bank_account_number { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<book> books { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<purchase_order> purchase_order { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<supplier_debt> supplier_debt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<purchase_payment> purchase_payment { get; set; }
    }
}
