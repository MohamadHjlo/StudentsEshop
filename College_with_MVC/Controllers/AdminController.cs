using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace College_with_MVC.Models
{
    public class AdminController : Controller
    {
        private EShop_Context _context;

        public AdminController()
        {
            _context = new EShop_Context();
        }

        public ActionResult Index()
        {
            if (Session["Admin"] == null) return Redirect("../Home");

            var users = _context.Users.ToList();
            return View(users);
        }
        public ActionResult SearchPage(string productsearchkey, string ordersearchKey)
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return Redirect("../");
            }
            var model = new UserSearchViewModel();

            if (productsearchkey != null)
            {
                var products = _context.Products.Where(p => p.Name.Contains(productsearchkey) || p.Describtion.Contains(productsearchkey)).Include(p => p.OrderDetails).ToList();
                model.Products = products;

            }
            if (ordersearchKey != null)
            {
                
                var orders = _context.Orders.ToList().Where(o => o.OrderID.ToString()
                    .Contains(ordersearchKey)).AsQueryable().Include(p => p.OrderDetails).ToList();
                model.Orders = orders;
            }
            return View(model);
        }
        public ActionResult AddUser()
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(AddEditUserViewModel model)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    Username = model.UserName,
                    Password = model.Password
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public ActionResult EditUser(int userId)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            if (userId > 0)
            {
                var user = _context.Users.FirstOrDefault(u => u.ID == userId);

                if (user != null)
                {
                    var edituser = new AddEditUserViewModel()
                    {
                        UserName = user.Username,
                        Password = user.Password,
                        Id = user.ID
                    };
                    return View(edituser);
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult EditUser(AddEditUserViewModel model)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(p => p.ID == model.Id);
                if (user == null) return View(model);
                user.Username = model.UserName;
                user.Password = model.Password;
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult DeleteUser(int userId)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            var user = _context.Users.FirstOrDefault(u => u.ID == userId);
            if (user != null)
            {
                var relatedOrder = _context.Orders.Where(d => d.UserID == userId).Include(d => d.OrderDetails).ToList();
                foreach (var order in relatedOrder)
                {
                    foreach (var relatedorderOrderDetail in order.OrderDetails.ToList())
                    {
                        _context.OrderDetails.Remove(relatedorderOrderDetail);
                    }
                    _context.Orders.Remove(order);
                }
                _context.Users.Remove(user);
                _context.SaveChanges();

                ViewData["SuccessDelete"] = $" کاربر {user.Username} با موفقیت حذف شد";
            }
            return View("Index", _context.Users.ToList());
        }

        public ActionResult Products()
        {
            if (Session["Admin"] == null) return Redirect("../Home");

            var products = _context.Products.ToList();
            return View(products);
        }
        public ActionResult AddProduct()
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            return View();
        }
        [HttpPost]
        public ActionResult AddProduct(AddEditProductVieModel model)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            if (ModelState.IsValid)
            {
                var product = new Product()
                {
                    Name = model.ProductName,
                    Price = (decimal)model.Price,
                    Describtion = model.Describtion
                };
                _context.Products.Add(product);
                _context.SaveChanges();

                if (model.File?.ContentLength > 0)
                {
                    var ext = Path.GetExtension(model.File.FileName);
                    string filepath = Path.Combine(Server.MapPath("~/Images/")
                    , product.ProductID + Path.ChangeExtension(ext, ".jpg"));

                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        model.File.InputStream.CopyTo(stream);
                    }

                    return RedirectToAction("Products");
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult EditProduct(int productId)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            if (productId > 0)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductID == productId);

                if (product != null)
                {
                    var editproduct = new AddEditProductVieModel()
                    {
                        ProductName = product.Name,
                        Price = (float)product.Price,
                        Describtion = product.Describtion,
                        Id = product.ProductID
                    };
                    return View(editproduct);
                }
                return RedirectToAction("Products");
            }
            return RedirectToAction("Products");
        }
        [HttpPost]
        public ActionResult EditProduct(AddEditProductVieModel model)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            if (ModelState.IsValid)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductID == model.Id);
                if (product == null) return View(model);
                product.Name = model.ProductName;
                product.Price = (long)model.Price;
                product.Describtion = model.Describtion;
                _context.SaveChanges();

                if (model.File?.ContentLength > 0)
                {
                    var ext = Path.GetExtension(model.File.FileName);
                    string filepath = Path.Combine(Server.MapPath("~/Images/")
                        , product.ProductID + Path.ChangeExtension(ext, ".jpg"));

                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        model.File.InputStream.CopyTo(stream);
                    }

                    return RedirectToAction("Products");
                }
            }

            return View(model);
        }

        public ActionResult DeleteProduct(int productId)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            var product = _context.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                var relatedorders = _context.OrderDetails.Where(d => d.ProductID == productId).ToList();
                foreach (var relatedorder in relatedorders)
                {
                    _context.OrderDetails.Remove(relatedorder);
                }

                _context.Products.Remove(product);
                _context.SaveChanges();
                string filepath = Path.Combine(Server.MapPath("~/Images/")
                    , product.ProductID + ".jpg");
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                ViewData["SuccessDelete"] = $" محصول {product.Name} با موفقیت حذف شد";
            }
            return View("Products", _context.Products.ToList());
        }

        public ActionResult Orders()
        {
            if (Session["Admin"] == null) return Redirect("../Home");

            var orders = _context.Orders.Include(o => o.User).ToList();
            return View(orders);
        }
        public ActionResult AddOrder()
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            return View();
        }
        [HttpPost]
        public ActionResult AddOrder(AddEditOrderViewModel model)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            if (ModelState.IsValid)
            {
                if (!_context.Users.Any(u => u.ID == model.UserId))
                {
                    ViewData["NotFoundSuchUser"] = "چنین کاربری با این شناسه وجود ندارد";
                    return View(model);
                }
                if (!_context.Products.Any(p => p.ProductID == model.ProductId))
                {
                    ViewData["NotFoundSuchProduct"] = "چنین محصولی با این شناسه وجود ندارد";
                    return View(model);
                }
                var order = new Order()
                {
                    CreatDate = DateTime.Now,
                    UserID = model.UserId,
                    IsFinaly = model.IsFinaly
                };
                _context.Orders.Add(order);
                _context.SaveChanges();

                var price = _context.Products.FirstOrDefault(p => p.ProductID == model.ProductId).Price;

                var detail = new OrderDetail()
                {
                    OrderID = order.OrderID,
                    Count = model.Count,
                    ProductID = model.ProductId,
                    Price = price
                };
                _context.OrderDetails.Add(detail);
                _context.SaveChanges();
                ViewData["SuccessMessage"] = "فاکتور با موفقیت اضافه شد";
                return View("Orders", _context.Orders.Include(o => o.User).ToList());
            }

            return View();
        }

        [HttpGet]
        public ActionResult ViewOrderDetails(int orderId)
        {
            if (Session["Admin"] == null) return Redirect("");
            if (orderId > 0)
            {
                var orderdetails = _context.OrderDetails.AsQueryable()
                    .Include(d => d.Order).Include(d => d.Product).Where(o => o.OrderID == orderId).ToList();
                ViewBag.UserId = _context.Orders.FirstOrDefault(o => o.OrderID == orderId).UserID;
                ViewBag.OrderId = orderId;
                return View(orderdetails);
            }
            return RedirectToAction("Orders");
        }
        public ActionResult AddDetail(int userId, int orderId)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            var model = new AddEditOrderViewModel() { UserId = userId, OrderID = orderId };
            return View(model);
        }
        [HttpPost]
        public ActionResult AddDetail(AddEditOrderViewModel model)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            if (ModelState.IsValid)
            {
                if (!_context.Users.Any(u => u.ID == model.UserId))
                {
                    ViewData["NotFoundSuchUser"] = "چنین کاربری با این شناسه وجود ندارد";
                    return View(model);
                }
                if (!_context.Products.Any(p => p.ProductID == model.ProductId))
                {
                    ViewData["NotFoundSuchProduct"] = "چنین محصولی با این شناسه وجود ندارد";
                    return View(model);
                }

                var price = _context.Products.FirstOrDefault(p => p.ProductID == model.ProductId).Price;

                var detail = new OrderDetail()
                {
                    OrderID = model.OrderID,
                    Count = model.Count,
                    ProductID = model.ProductId,
                    Price = price
                };
                _context.OrderDetails.Add(detail);
                _context.SaveChanges();
                ViewData["SuccessMessage"] = "ریز فاکتور با موفقیت اضافه شد";
                return Redirect($"ViewOrderDetails/?orderId={model.OrderID}");
            }

            return View();
        }

        public ActionResult EditDetail(int detailId)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            var detail = _context.OrderDetails.Include(d => d.Order).FirstOrDefault(d => d.DetailID == detailId);
            var model = new AddEditOrderViewModel() { UserId = detail.Order.UserID, OrderID = detail.OrderID, DetailId = detailId, ProductId = detail.ProductID, Count = detail.Count };
            return View(model);
        }
        [HttpPost]
        public ActionResult EditDetail(AddEditOrderViewModel model)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            if (ModelState.IsValid)
            {
                if (!_context.Users.Any(u => u.ID == model.UserId))
                {
                    ViewData["NotFoundSuchUser"] = "چنین کاربری با این شناسه وجود ندارد";
                    return View(model);
                }
                if (!_context.Products.Any(p => p.ProductID == model.ProductId))
                {
                    ViewData["NotFoundSuchProduct"] = "چنین محصولی با این شناسه وجود ندارد";
                    return View(model);
                }

                var price = _context.Products.FirstOrDefault(p => p.ProductID == model.ProductId).Price;

                var detail = _context.OrderDetails.FirstOrDefault(d => d.DetailID == model.DetailId);

                detail.OrderID = model.OrderID;
                detail.Count = model.Count;
                detail.ProductID = model.ProductId;
                detail.Price = price;

                _context.SaveChanges();
                ViewData["SuccessMessage"] = "ریز فاکتور با موفقیت تغییر پیدا کرد";
                return Redirect($"ViewOrderDetails/?orderId={model.OrderID}");
            }

            return View();
        }
        public ActionResult DeleteDetail(int detailId)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            var detail = _context.OrderDetails.FirstOrDefault(d => d.DetailID == detailId);
            if (detail != null)
            {
                _context.OrderDetails.Remove(detail);
                _context.SaveChanges();

                ViewData["SuccessDelete"] = $" ریز فاکتور با شناسه {detail.DetailID} با موفقیت حذف شد";

                return Redirect($"ViewOrderDetails/?orderId={detail.OrderID}");
            }

            return RedirectToAction("ViewOrderDetails");
        }

        public ActionResult DeleteOrder(int orderId)
        {
            if (Session["Admin"] == null) return Redirect("../Home");
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == orderId);
            if (order != null)
            {
                var details = _context.OrderDetails.Where(d => d.OrderID == orderId).ToList();
                foreach (var detail in details)
                {
                    _context.OrderDetails.Remove(detail);
                }
                _context.Orders.Remove(order);
                _context.SaveChanges();
                ViewData["SuccessMessage"] = $" فاکتور با شناسه {order.OrderID} با موفقیت حذف شد";
            }
            return View("Orders", _context.Orders.ToList());
        }

    }
}