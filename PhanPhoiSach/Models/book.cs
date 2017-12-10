namespace PhanPhoiSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("book")]
    public partial class book
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public book()
        {
            agency_debt_item = new HashSet<agency_debt_item>();
            purchase_order_item = new HashSet<purchase_order_item>();
            purchase_payment_item = new HashSet<purchase_payment_item>();
            sale_order_item = new HashSet<sale_order_item>();
            sale_payment_item = new HashSet<sale_payment_item>();
            stores = new HashSet<store>();
            supplier_debt_item = new HashSet<supplier_debt_item>();
        }

        [Key]

        [Display(Name = "Mã sách")]
        public int book_id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên sách")]
        public string book_name { get; set; }

        [Display(Name = "Tác giả")]
        public string book_author { get; set; }

        [Display(Name = "Lĩnh vực")]
        public string book_field { get; set; }

        [Display(Name = "Mã NXB")]
        public int fk_supplier { get; set; }

        [Display(Name = "Số lượng")]
        public int? book_stock { get; set; }

        [Display(Name = "Giá bán")]
        public decimal? book_seling_price { get; set; }

        [Display(Name = "Giá nhập")]
        public decimal? book_purchase_price { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<agency_debt_item> agency_debt_item { get; set; }

        public virtual supplier supplier { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<purchase_order_item> purchase_order_item { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<purchase_payment_item> purchase_payment_item { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sale_order_item> sale_order_item { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sale_payment_item> sale_payment_item { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<store> stores { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<supplier_debt_item> supplier_debt_item { get; set; }
    }
}
