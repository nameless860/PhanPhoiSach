using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhanPhoiSach.Models;

namespace PhanPhoiSach.ViewModels
{
    public class revenue_statistics
    {
        public int book_id { get; set; }
        public string book_name { get; set; }
        public int? quantity { get; set; }
        public decimal? total_money { get; set; }
    }
}