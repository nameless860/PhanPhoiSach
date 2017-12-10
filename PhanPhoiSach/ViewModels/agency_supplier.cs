using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhanPhoiSach.Models;

namespace PhanPhoiSach.ViewModels
{
    public class agency_supplier
    {
        public List<agency_debt> a_debt { get; set; }
        public List<supplier_debt> s_debt { get; set; }
    }
}