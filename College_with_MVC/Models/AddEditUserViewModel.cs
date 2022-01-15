using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace College_with_MVC.Models
{
    public class AddEditUserViewModel
    {
        
        public int Id { get; set; }


        [DisplayName("نام کاربری")]
        [Required(ErrorMessage = "نام کاربری باید وارد شود")]
        public string UserName { get; set; }


        [DisplayName("رمز عبور")]
        [Required(ErrorMessage = "رمز عبور کاربر باید وارد شود")]
        public string Password { get; set; } 
            

    }
}