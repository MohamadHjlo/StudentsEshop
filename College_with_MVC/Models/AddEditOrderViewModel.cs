using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace College_with_MVC.Models
{
    public class AddEditOrderViewModel
    {
        public int OrderID { get; set;}

        public int DetailId { get; set; }

        [DisplayName("شناسه محصول مربوطه")]
        [Required(ErrorMessage = "شناسه محصول باید وارد شود")]
        public int ProductId { get; set; }
       
        [DisplayName("شناسه کاربر مربوطه")]
        [Required(ErrorMessage = "شناسه کاربر باید وارد شود")]
        public int UserId { get; set; }

        [DisplayName("تعداد")]
        [Required(ErrorMessage = "تعداد باید وارد شود")]
        public int Count { get; set; }

        [DisplayName("ثبت کردن با عنوان نهایی شده")]
        public bool IsFinaly { get; set; }
    }
}