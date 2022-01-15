using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace College_with_MVC.Models
{
    public class UserSearchViewModel
    {
        public List<Product> Products { get; set; }

        public List<Order> Orders { get; set; }
    }
}