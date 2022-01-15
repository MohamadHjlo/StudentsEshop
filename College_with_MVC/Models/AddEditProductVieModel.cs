using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace College_with_MVC.Models
{
    public class AddEditProductVieModel
    {
        [DisplayName("نام")]
        [Required(ErrorMessage = "نام محصول باید وارد شود")]
        public string ProductName { get; set; }

        public int Id { get; set; }

        [DisplayName("تصویر")]
        [Required(ErrorMessage = "تصویر محصول باید وارد شود")]
        public HttpPostedFileBase File { get; set; }

        [DisplayName("قیمت")]
        [Required(ErrorMessage = "قیمت محصول باید وارد شود")]
        public float Price { get; set; }

        [DisplayName("توضیحات")]
        [Required(ErrorMessage = "توضیحات محصول باید وارد شود")]
        public string Describtion { get; set; }

    }
}