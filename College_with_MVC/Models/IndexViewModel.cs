using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace College_with_MVC.Models
{
    public class IndexViewModel
    {
        
        [Required(ErrorMessage ="نام  کاربری خود را وارد کنید")]
        [DisplayName("نام کاربری")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "رمز عبور خود را وارد کنید")]
        [DisplayName("رمز عبور")]
        
        public string Password { get; set; } 

            
    }
}