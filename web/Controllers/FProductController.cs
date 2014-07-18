using BLL.PhotoBL;
using BLL.ProductBL;
using BLL.ReferenceBL;
using DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Models;

namespace web.Controllers
{
    public class FProductController : Controller
    {
        public ActionResult Index(int id)
        {
            //web.Areas.Admin.Models.VMProductGroupModel grouplist = new web.Areas.Admin.Models.VMProductGroupModel();
            //grouplist.ProductGroup = ProductManager.GetProductGroupList("tr");
            int pId = Convert.ToInt32(RouteData.Values["id"]);
         //   ViewBag.ProductGroup = ProductManager.GetProductGroupItem(pId);
            ViewData["referanslar"] = ReferenceManager.GetReferenceListForFront("tr");
            
            
            
            using(MainContext db = new MainContext())
            {
                ProductFrontModel model = new ProductFrontModel();
                model.product = ProductManager.GetProductById(pId);
                int catId = model.product.ProductGroupId;
                model.headers = db.ProductHeaders.FirstOrDefault(x=>x.CategoryId==catId);
                model.ProductInfo = db.ProductInformation.Where(x => x.ProductId == pId).ToList();

                ViewBag.ProductGroup = model.product.ProductGroup.GroupName;
                ViewBag.Photos = PhotoManager.GetListForFront(11, pId);

                ViewBag.ProductInfo = db.ProductDetail.Where(x => x.ProductId == pId).ToList();
                return View(model);
            }
               


          
        }

        public ActionResult Prices()
        {
            if (RouteData.Values["id"] != null)
            {
                int pId = Convert.ToInt32(RouteData.Values["id"]);
                ViewBag.ProductGroup = ProductManager.GetProductGroupItem(pId);


                if (Session["userlogin"] != null)
                    ViewBag.userloginemail = Session["userlogin"];
                else ViewBag.userloginemail = "";

                return View(ProductManager.GetProductListFront(pId).ToList());
            }
            return View();
        }

        [HttpGet]
        public ActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserLogin(UserModel user,string submit)
        {
            if (submit == "Giriş")
            {
                if (ProductManager.GetLoginControl(user.Login.Email, user.Login.Password))
                {
                    Session["userlogin"] = user.Login.Email;
                    return RedirectToAction("Index", "FHome");
                }
                else TempData["login"] = "false";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (ProductManager.GetMailControl(user.Login.Email))
                    {
                        if (user.NewUser.Password == user.RePassword)
                        {
                            if (ProductManager.AddUser(user.NewUser))
                            {
                                Session["userlogin"] = user.NewUser.Email;
                                return RedirectToAction("Index", "FHome");
                            }
                            else TempData["usernew"] = "false";
                        }
                        else TempData["pass"] = "false";
                    }
                    else TempData["mailisuse"] = "false";
                }
                else TempData["usernewvalid"] = "false"; 
            }


            return View();
        }

        public ActionResult UserLogout()
        {
            if (Session["userlogin"] != null) Session.Remove("userlogin");
            return RedirectToAction("Index", "FHome");
        }
        

    }
}
