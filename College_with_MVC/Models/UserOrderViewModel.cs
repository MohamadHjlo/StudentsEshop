using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace College_with_MVC.Models
{
    public class UserOrderViewModel
    {
        public Order Order { get; set; }
        public List<Order> Orders { get; set; }
        public List<OrderDetail> Details { get; set; }
    }
}