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
using web.Areas.Admin.Models;

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
            
            web.Areas.Admin.Models.VMProductGroupModel grouplist = new Models.VMProductGroupModel();
            grouplist.ProductGroup = ProductManager.GetProductGroupList("tr");
            ProductAddModel model = new ProductAddModel();
            model.VMProductGroupModel = grouplist;
      //      ViewBag.Groups = grouplist;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddProduct(ProductAddModel model, IEnumerable<HttpPostedFileBase> attachments, HttpPostedFileBase prd1, HttpPostedFileBase prd2)
        {
            try
            {
                model.Product.PageSlug = Utility.SetPagePlug(model.Product.Name);

                model.Product.ProductGroupId = 1;

                if (prd1 != null)
                {
                    //prd1.SaveAs(Server.MapPath("/Content/images/userfiles/") + item.FileName);
                    Random random = new Random();
                    int rand = random.Next(1000, 99999999);
                    string path = Utility.SetPagePlug(model.Product.Name) + "_" + rand + Path.GetExtension(prd1.FileName);
                    new ImageHelper(1020, 768).SaveThumbnail(prd1, "/Content/images/userfiles/", path);
                    model.Product.Image1 = "/Content/images/userfiles/" + path;
                }
                else
                {
                //    model.Product.Image1 = "/Content/images/front/noimage.jpeg";
                }

                if (prd2 != null)
                {
                    //prd1.SaveAs(Server.MapPath("/Content/images/userfiles/") + item.FileName);
                    Random random = new Random();
                    int rand = random.Next(1000, 99999999);
                    string path = Utility.SetPagePlug(model.Product.Name) + "_" + rand + Path.GetExtension(prd1.FileName);
                    new ImageHelper(1020, 768).SaveThumbnail(prd1, "/Content/images/userfiles/", path);
                    model.Product.Image2 = "/Content/images/userfiles/" + path;
                }
                else
                {
                 //   model.Product.Image2 = "/Content/images/front/noimage.jpeg";
                }

                ProductManager.AddProduct(model.Product);

                foreach (var item in attachments)
                {
                    if (item != null && item.ContentLength > 0)
                    {
                        item.SaveAs(Server.MapPath("/Content/images/userfiles/") + item.FileName);
                        Random random = new Random();
                        int rand = random.Next(1000, 99999999);
                        string path = Utility.SetPagePlug(model.Product.Name) + "_" + rand + Path.GetExtension(item.FileName);
                        new ImageHelper(1020, 768).SaveThumbnail(item, "/Content/images/userfiles/", path);

                        rand = random.Next(1000, 99999999);
                        string thumbnail = Utility.SetPagePlug(model.Product.Name) + "_" + rand + Path.GetExtension(item.FileName);
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
                        p.CategoryId = (int)PhotoType.ProductUygulama;
                        p.ItemId = model.Product.ProductId;
                        p.Path = "/Content/images/userfiles/" + path;
                        p.Thumbnail = "/Content/images/userfiles/" + thumbnail;
                        p.Online = true;
                        p.SortOrder = 9999;
                        p.Language = "tr";
                        p.TimeCreated = DateTime.Now;
                        p.Title = model.Product.Name;
                        PhotoManager.Save(p);
                       
                    }
                }

             
                ViewBag.SaveResult = true;
            }
            catch (Exception ex)
            {
                ViewBag.SaveResult = false;
            }



            return Redirect("/yonetim/urunekle/" + model.Product.ProductId);
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
            using (MainContext db = new MainContext())
            {
                var prd = db.Product.Where(x => x.Deleted==false).ToList();
                return View(prd);
            }
          
        }

        public void SaveCategory(int id, int prdId)
        {
            using (MainContext db = new MainContext())
            {
                Product prd = db.Product.Where(x => x.ProductId == prdId).SingleOrDefault();
                if (prd != null)
                {
                    prd.ProductGroupId = id;
                    db.SaveChanges();
                }
            }
        }

        [AllowAnonymous]
        public JsonResult DeleteProduct(int id)
        {
            using (MainContext db = new MainContext())
            {
                try
                {
                    var record = db.Product.FirstOrDefault(d => d.ProductId == id);
                    db.Product.Remove(record);
                    db.SaveChanges();
                    return Json(true);
                }
                catch (Exception)
                {
                    return Json(false);
                }
            }
        }


        public JsonResult SortRecords(string list)
        {
            JsonList psl = (new JavaScriptSerializer()).Deserialize<JsonList>(list);
            string[] idsList = psl.list;
            using (MainContext db = new MainContext())
            {
                try
                {

                    int row = 0;
                    foreach (string id in idsList)
                    {
                        int mid = Convert.ToInt32(id);
                        Product sortingrecord = db.Product.SingleOrDefault(d => d.ProductId == mid);
                        sortingrecord.SortNumber = Convert.ToInt32(row);
                        db.SaveChanges();
                        row++;
                    }
                    return Json(true);
                }
                catch (Exception)
                {
                    return Json(false);
                }
            }



        }

        public class JsonList
        {
            public string[] list { get; set; }
        }
    }
}
