using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using College_with_MVC.Models;

namespace College_with_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly EShop_Context _context;
        public HomeController()
        {
            _context = new EShop_Context();
        }
        public ActionResult Index()
        {

            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("UserLogin");
            }

            return View(_context.Products.ToList());
        }
        public ActionResult Details(int productId)
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("UserLogin");
            }

            var product = _context.Products.FirstOrDefault(p => p.ProductID == productId);
            return View(product);
        }

        public ActionResult SearchPage(string productsearchkey, string ordersearchKey)
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            var model = new UserSearchViewModel();

            if (productsearchkey != null)
            {
                var user = Session["user"] as User;
                var products = _context.Products.Where(p => p.Name.Contains(productsearchkey) || p.Describtion.Contains(productsearchkey)).Include(p => p.OrderDetails).ToList();
                model.Products = products;

            }
            if (ordersearchKey != null)
            {
                var user = Session["user"] as User;
                var orders = _context.Orders.ToList().Where(o => o.OrderID.ToString()
                    .Contains(ordersearchKey) && o.UserID == user.ID).AsQueryable().Include(p => p.OrderDetails).ToList();
                model.Orders = orders;


            }
            return View(model);
        }


        public ActionResult AddToUserOrders(int productId)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            var user = Session["User"] as User;

            var product = _context.Products.FirstOrDefault(p => p.ProductID == productId);
            if (!_context.Orders.Any(o => o.UserID == user.ID && o.IsFinaly != true))
            {
                var order = new Order()
                {
                    UserID = user.ID,
                    CreatDate = DateTime.Now,
                    IsFinaly = false
                };
                _context.Orders.Add(order);
                _context.SaveChanges();
                _context.OrderDetails.Add(new OrderDetail()
                {
                    OrderID = order.OrderID,
                    ProductID = product.ProductID,
                    Price = product.Price,
                    Count = 1
                });
                _context.SaveChanges();
            }
            else
            {
                var order = _context.Orders.FirstOrDefault(o => o.UserID == user.ID && o.IsFinaly != true);
                var orderDetail = _context.OrderDetails.FirstOrDefault(d => d.OrderID == order.OrderID && d.ProductID == productId);
                if (orderDetail != null)
                {
                    orderDetail.Count++;
                }
                else
                {
                    _context.OrderDetails.Add(new OrderDetail()
                    {
                        OrderID = order.OrderID,
                        ProductID = product.ProductID,
                        Price = product.Price,
                        Count = 1
                    });
                }
                _context.SaveChanges();
            }

            return RedirectToAction("ShowUserOrders");
        }
        public ActionResult ShowUserOrders(int? messagecheck)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("UserLogin");
            }

            if (messagecheck != null)
            {
                ViewData["Message"] = "ثبت با موفقیت انجام شد";
            }
            var user = Session["User"] as User;
            int userId = user.ID;

            var order = _context.Orders
                .Where(o => o.UserID == userId && o.IsFinaly == false)
                .Include(o => o.OrderDetails).FirstOrDefault();

            if (order != null)
            {
                var model = new UserOrderViewModel
                {
                    Order = order,
                    Details = order.OrderDetails.AsQueryable().Include(d => d.Product).ToList()
                };

                return View(model);
            }

            return View();
        }

        public ActionResult RemoveItem(int detailId)
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            var factorDetail = _context.OrderDetails.Find(detailId);
            if (factorDetail != null) _context.OrderDetails.Remove(factorDetail);
            _context.SaveChanges();
            return RedirectToAction("ShowUserOrders");
        }
        public ActionResult SubmitAsFinalyBuy(int orderId)
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("UserLogin");
            }
            var order = _context.Orders.Find(orderId);

            if (order != null)
            {
                order.IsFinaly = true;
            }
            _context.SaveChanges();

            return RedirectToAction("ShowUserOrders", "Home", new { messagecheck = 1 });
        }
        public ActionResult UserSignIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserSignIn(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Username == model.UserName))
                {
                    ViewData["ValidateError"] = "این نام قبلا توسط شخصی دیگر بکار رفته است";
                    return View();
                }
                var user = new User()
                {
                    Username = model.UserName,
                    Password = model.Password
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                Session["User"] = user;
                return View("Submit_Response", model);
            }
            return View(model);
        }

        public ActionResult UserLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserLogin(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!_context.Users.Any(u => u.Password == model.Password && u.Username == model.UserName))
                {
                    ViewData["ValidateError"] = "خطا... چنین کاربری وجود ندارد";
                    return View();
                }
                var user = _context.Users.FirstOrDefault(u => u.Username == model.UserName && u.Password == model.Password);
                ViewData["ValidateError"] = "";
                Session["User"] = user;
                return RedirectToAction("Index");
            }
            ViewData["ValidateError"] = "خطا... چنین کاربری وجود ندارد";
            return View(model);
        }


        public ActionResult AdminSignIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminSignIn(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Username == model.UserName))
                {
                    ViewData["ValidateAdminError"] = "این نام قبلا توسط شخصی دیگر بکار رفته است";
                    return View();
                }
                var admin = new User()
                {
                    Username = model.UserName,
                    Password = model.Password,
                    IsAdmin = true
                };
                _context.Users.Add(admin);
                _context.SaveChanges();
                Session["Admin"] = admin;
                return View("Submit_Response", model);
            }
            return View(model);
        }

        public ActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminLogin(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!_context.Users.Any(u => u.Password == model.Password && u.Username == model.UserName&&u.IsAdmin==true))
                {
                    ViewData["ValidateAdminError"] = "خطا...اطلاعات نادرست است";
                    return View();
                }
                var admin = _context.Users.FirstOrDefault(u => u.Username == model.UserName && u.Password == model.Password);
                ViewData["ValidateAdminError"] = "";
                Session["Admin"] = admin;
                return Redirect("../Admin/Index");
            }

            ViewData["ValidateAdminError"] = "خطا...اطلاعات نادرست است";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}