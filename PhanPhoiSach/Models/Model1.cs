namespace PhanPhoiSach.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<agency> agencies { get; set; }
        public virtual DbSet<agency_debt> agency_debt { get; set; }
        public virtual DbSet<agency_debt_item> agency_debt_item { get; set; }
        public virtual DbSet<book> books { get; set; }
        public virtual DbSet<purchase_order> purchase_order { get; set; }
        public virtual DbSet<purchase_order_item> purchase_order_item { get; set; }
        public virtual DbSet<purchase_payment> purchase_payment { get; set; }
        public virtual DbSet<purchase_payment_item> purchase_payment_item { get; set; }
        public virtual DbSet<sale_order> sale_order { get; set; }
        public virtual DbSet<sale_order_item> sale_order_item { get; set; }
        public virtual DbSet<sale_payment> sale_payment { get; set; }
        public virtual DbSet<sale_payment_item> sale_payment_item { get; set; }
        public virtual DbSet<store> stores { get; set; }
        public virtual DbSet<supplier> suppliers { get; set; }
        public virtual DbSet<supplier_debt> supplier_debt { get; set; }
        public virtual DbSet<supplier_debt_item> supplier_debt_item { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<agency>()
                .Property(e => e.agency_phone)
                .IsUnicode(false);

            modelBuilder.Entity<agency>()
                .HasMany(e => e.agency_debt)
                .WithRequired(e => e.agency)
                .HasForeignKey(e => e.fk_agency)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<agency>()
                .HasMany(e => e.sale_order)
                .WithRequired(e => e.agency)
                .HasForeignKey(e => e.fk_agency)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<agency_debt>()
                .Property(e => e.agency_debt_total_money)
                .HasPrecision(14, 0);

            modelBuilder.Entity<agency_debt>()
                .HasMany(e => e.agency_debt_item)
                .WithRequired(e => e.agency_debt)
                .HasForeignKey(e => e.fk_agency_debt)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<agency_debt_item>()
                .Property(e => e.agency_debt_item_money)
                .HasPrecision(14, 0);

            modelBuilder.Entity<book>()
                .Property(e => e.book_seling_price)
                .HasPrecision(14, 0);

            modelBuilder.Entity<book>()
                .Property(e => e.book_purchase_price)
                .HasPrecision(14, 0);

            modelBuilder.Entity<book>()
                .HasMany(e => e.agency_debt_item)
                .WithRequired(e => e.book)
                .HasForeignKey(e => e.fk_book)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<book>()
                .HasMany(e => e.purchase_order_item)
                .WithRequired(e => e.book)
                .HasForeignKey(e => e.fk_book)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<book>()
                .HasMany(e => e.purchase_payment_item)
                .WithRequired(e => e.book)
                .HasForeignKey(e => e.fk_book)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<book>()
                .HasMany(e => e.sale_order_item)
                .WithRequired(e => e.book)
                .HasForeignKey(e => e.fk_book)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<book>()
                .HasMany(e => e.sale_payment_item)
                .WithRequired(e => e.book)
                .HasForeignKey(e => e.fk_book)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<book>()
                .HasMany(e => e.stores)
                .WithRequired(e => e.book)
                .HasForeignKey(e => e.fk_book)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<book>()
                .HasMany(e => e.supplier_debt_item)
                .WithRequired(e => e.book)
                .HasForeignKey(e => e.fk_book)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<purchase_order>()
                .Property(e => e.purchase_order_total_money)
                .HasPrecision(14, 0);

            modelBuilder.Entity<purchase_order>()
                .HasMany(e => e.purchase_order_item)
                .WithRequired(e => e.purchase_order)
                .HasForeignKey(e => e.fk_purchase_order)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<purchase_order>()
            //    .HasMany(e => e.purchase_payment)
            //    .WithRequired(e => e.purchase_order)
            //    .HasForeignKey(e => e.fk_purchase_order)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<supplier>()
                .HasMany(e => e.purchase_payment)
                .WithRequired(e => e.supplier)
                .HasForeignKey(e => e.fk_supplier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<purchase_order_item>()
                .Property(e => e.purchase_order_item_price)
                .HasPrecision(14, 0);

            modelBuilder.Entity<purchase_order_item>()
                .Property(e => e.purchase_order_money)
                .HasPrecision(14, 0);

            modelBuilder.Entity<purchase_payment>()
                .Property(e => e.purchase_payment_money_pay)
                .HasPrecision(14, 0);

            //modelBuilder.Entity<purchase_payment>()
            //    .Property(e => e.purchase_payment_debt)
            //    .HasPrecision(14, 0);

            modelBuilder.Entity<purchase_payment>()
                .HasMany(e => e.purchase_payment_item)
                .WithRequired(e => e.purchase_payment)
                .HasForeignKey(e => e.fk_purchase_payment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<purchase_payment_item>()
                .Property(e => e.purchase_payment_item_price)
                .HasPrecision(14, 0);

            modelBuilder.Entity<purchase_payment_item>()
                .Property(e => e.purchase_payment_item_money)
                .HasPrecision(14, 0);

            modelBuilder.Entity<sale_order>()
                .Property(e => e.sale_order_total_money)
                .HasPrecision(14, 0);

            modelBuilder.Entity<sale_order>()
                .HasMany(e => e.sale_order_item)
                .WithRequired(e => e.sale_order)
                .HasForeignKey(e => e.fk_sale_order)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<sale_order>()
            //    .HasMany(e => e.sale_payment)
            //    .WithRequired(e => e.sale_order)
            //    .HasForeignKey(e => e.fk_sale_order)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<agency>()
                .HasMany(e => e.sale_payment)
                .WithRequired(e => e.agency)
                .HasForeignKey(e => e.fk_agency)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<sale_order_item>()
                .Property(e => e.sale_order_item_price)
                .HasPrecision(14, 0);

            modelBuilder.Entity<sale_order_item>()
                .Property(e => e.sale_order_money)
                .HasPrecision(14, 0);

            modelBuilder.Entity<sale_payment>()
                .Property(e => e.sale_payment_money_pay)
                .HasPrecision(14, 0);

            //modelBuilder.Entity<sale_payment>()
            //    .Property(e => e.sale_payment_debt)
            //    .HasPrecision(14, 0);

            modelBuilder.Entity<sale_payment>()
                .HasMany(e => e.sale_payment_item)
                .WithRequired(e => e.sale_payment)
                .HasForeignKey(e => e.fk_sale_payment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<sale_payment_item>()
                .Property(e => e.sale_payment_item_price)
                .HasPrecision(14, 0);

            modelBuilder.Entity<sale_payment_item>()
                .Property(e => e.sale_payment_item_money)
                .HasPrecision(14, 0);

            modelBuilder.Entity<store>()
                .Property(e => e.store_seling_price)
                .HasPrecision(14, 0);

            modelBuilder.Entity<store>()
                .Property(e => e.store_purchase_price)
                .HasPrecision(14, 0);

            modelBuilder.Entity<supplier>()
                .Property(e => e.supplier_phone)
                .IsUnicode(false);

            modelBuilder.Entity<supplier>()
                .HasMany(e => e.books)
                .WithRequired(e => e.supplier)
                .HasForeignKey(e => e.fk_supplier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<supplier>()
                .HasMany(e => e.purchase_order)
                .WithRequired(e => e.supplier)
                .HasForeignKey(e => e.fk_supplier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<supplier>()
                .HasMany(e => e.supplier_debt)
                .WithRequired(e => e.supplier)
                .HasForeignKey(e => e.fk_supplier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<supplier_debt>()
                .Property(e => e.supplier_debt_total_money)
                .HasPrecision(14, 0);

            modelBuilder.Entity<supplier_debt>()
                .HasMany(e => e.supplier_debt_item)
                .WithRequired(e => e.supplier_debt)
                .HasForeignKey(e => e.fk_supplier_debt)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<supplier_debt_item>()
                .Property(e => e.supplier_debt_item_money)
                .HasPrecision(14, 0);
        }
    }
}
