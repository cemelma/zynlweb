using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using web.Areas.Admin.Filters;
using web.Areas.Admin.Helpers;
using BLL.LanguageBL;
using BLL.ProductBL;
//using BLL.ProductBL;
using DAL.Entities;
using System.Drawing;
using BLL.PhotoBL;
using DAL.Context;

namespace web.Areas.Admin.Controllers
{
    [AuthenticateUser]
    public class ProductController : Controller
    {
        public ActionResult AddProduct(int id=0)
        {
            if (RouteData.Values["id"] != null)
            {
                ViewBag.SaveResult = true;
                ViewBag.ProductId = id;
            }
            else
            {
                ViewBag.SaveResult = false;
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddProduct(Product model, IEnumerable<HttpPostedFileBase> attachments, HttpPostedFileBase prd1, HttpPostedFileBase prd2)
        {
            try
            {
                model.PageSlug = Utility.SetPagePlug(model.Name);
                model.ProductGroupId = 1;
                ProductManager.AddProduct(model);

                foreach (var item in attachments)
                {
                    if (item != null && item.ContentLength > 0)
                    {
                        item.SaveAs(Server.MapPath("/Content/images/userfiles/") + item.FileName);
                        Random random = new Random();
                        int rand = random.Next(1000, 99999999);
                        string path = Utility.SetPagePlug(model.Name) + "_" + rand + Path.GetExtension(item.FileName);
                        new ImageHelper(1020, 768).SaveThumbnail(item, "/Content/images/userfiles/", path);

                        rand = random.Next(1000, 99999999);
                        string thumbnail = Utility.SetPagePlug(model.Name) + "_" + rand + Path.GetExtension(item.FileName);
                        Bitmap bmp = new Bitmap(Server.MapPath("/Content/images/userfiles/") + item.FileName);

                        Bitmap bmp2 = new Bitmap(bmp);

                        using (Bitmap Orgbmp = bmp2)
                        {

                            int sabit = 90;
                            Size Boyut = new Size(210, 125);
                            Bitmap ReSizedThmb = new Bitmap(Orgbmp, Boyut);
                            ReSizedThmb.Save(Server.MapPath("/Content/images/userfiles/") + thumbnail);
                            bmp.Dispose();
                            bmp2.Dispose();
                            Orgbmp.Dispose();
                            GC.Collect();
                        }

                        //new ImageHelper(300, 280).ResizeFromStream("/Content/images/userfiles/",thumbnail,img);
                        Photo p = new Photo();
                        p.CategoryId = (int)PhotoType.Product;
                        p.ItemId = model.ProductId;
                        p.Path = "/Content/images/userfiles/" + path;
                        p.Thumbnail = "/Content/images/userfiles/" + thumbnail;
                        p.Online = true;
                        p.SortOrder = 9999;
                        p.Language = "tr";
                        p.TimeCreated = DateTime.Now;
                        p.Title = model.Name;
                        PhotoManager.Save(p);
                       
                    }
                }

                //foreach (var item in attachmentsproduct)
                //{
                //    if (item != null && item.ContentLength > 0)
                //    {
                //        item.SaveAs(Server.MapPath("/Content/images/userfiles/") + item.FileName);
                //        Random random = new Random();
                //        int rand = random.Next(1000, 99999999);
                //        string path = Utility.SetPagePlug(model.Name) + "_" + rand + Path.GetExtension(item.FileName);
                //        new ImageHelper(1020, 768).SaveThumbnail(item, "/Content/images/userfiles/", path);

                //        rand = random.Next(1000, 99999999);
                //        string thumbnail = Utility.SetPagePlug(model.Name) + "_" + rand + Path.GetExtension(item.FileName);
                //        Bitmap bmp = new Bitmap(Server.MapPath("/Content/images/userfiles/") + item.FileName);

                //        Bitmap bmp2 = new Bitmap(bmp);

                //        using (Bitmap Orgbmp = bmp2)
                //        {

                //            int sabit = 90;
                //            Size Boyut = new Size(210, 125);
                //            Bitmap ReSizedThmb = new Bitmap(Orgbmp, Boyut);
                //            ReSizedThmb.Save(Server.MapPath("/Content/images/userfiles/") + thumbnail);
                //            bmp.Dispose();
                //            bmp2.Dispose();
                //            Orgbmp.Dispose();
                //            GC.Collect();
                //        }

                //        //new ImageHelper(300, 280).ResizeFromStream("/Content/images/userfiles/",thumbnail,img);
                //        Photo p = new Photo();
                //        p.CategoryId = (int)PhotoType.ProductUygulama;
                //        p.ItemId = model.ProductId;
                //        p.Path = "/Content/images/userfiles/" + path;
                //        p.Thumbnail = "/Content/images/userfiles/" + thumbnail;
                //        p.Online = true;
                //        p.SortOrder = 9999;
                //        p.Language = "tr";
                //        p.TimeCreated = DateTime.Now;
                //        p.Title = model.Name;
                //        PhotoManager.Save(p);
                        
                //    }
                //}
                ViewBag.SaveResult = true;
            }
            catch (Exception ex)
            {
                ViewBag.SaveResult = false;
            }


            //if (ModelState.IsValid)
            //{
            //    //ProductGroup model = new ProductGroup();
            //    //model.GroupName = txtname;
            //    //model.Language = "tr";
            //    //model.PageSlug = Utility.SetPagePlug(txtname);
            //    //model.TopProductId = topProductGroupId;
            //    //ViewBag.ProcessMessage = ProductManager.AddProductGroup(model);
            //    //web.Areas.Admin.Models.VMProductGroupModel grouplist = new Models.VMProductGroupModel();
            //    //grouplist.ProductGroup = ProductManager.GetProductGroupList(lang);
            //    //return View(grouplist);
            //}
            //return RedirectToAction("AddProduct", "Product", model.ProductId);
           return Redirect("/yonetim/urunekle/"+model.ProductId);
            //return View();
        }

        public ActionResult SaveDetail(string code, string malzeme, string birim, string ebat, string agirlik, string ton, string fiyat,string renk,string prId)
        {
            using (MainContext db = new MainContext())
            {
                if (!string.IsNullOrEmpty(prId))
                {
                    int id = Convert.ToInt32(prId);

                    ProductDetail pd = new ProductDetail();
                    pd.ProductId = id;
                    pd.Code = code;
                    pd.Material = malzeme;
                    pd.Unit = birim;
                    pd.VehicleTon = ton;
                    pd.Weight = agirlik;
                    pd.Dimension = ebat;
                    pd.Price = fiyat;
                    pd.ColorWhite = renk;


                    db.ProductDetail.Add(pd);
                    db.SaveChanges();

                    ViewBag.Details = db.ProductDetail.Where(x => x.ProductId == id).ToList();
                    return PartialView("~/Areas/Admin/Views/Product/_detailtable1.cshtml", ViewBag.Details);
                }
                else
                {
                    ViewBag.Details = null;
                    return PartialView("~/Areas/Admin/Views/Product/_detailtable1.cshtml", ViewBag.Details);
                }
            }
        }

        public ActionResult Index()
        {
            return View();
        }
       
    }
}
