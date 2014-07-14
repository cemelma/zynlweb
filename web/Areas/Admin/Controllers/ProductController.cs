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

        public ActionResult EditProduct(int id = 0)
        {
            web.Areas.Admin.Models.VMProductGroupModel grouplist = new Models.VMProductGroupModel();
            grouplist.ProductGroup = ProductManager.GetProductGroupList("tr");
            ProductAddModel model = new ProductAddModel();
            model.VMProductGroupModel = grouplist;

            if (RouteData.Values["id"] != null)
            {
                ViewBag.SaveResult = true;
                ViewBag.ProductId = id;
                Product prt = ProductManager.GetProductById(id);
                model.Product = prt;
            }
            else
            {
                ViewBag.SaveResult = false;
            }

            var photos = PhotoManager.GetList(11, id);
            ViewBag.Photos = photos;
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

                    prd1.SaveAs(Server.MapPath("/Content/images/userfiles/productbig/") + path);

                    new ImageHelper(210, 125).SaveThumbnail(prd1, "/Content/images/userfiles/", path);
                    model.Product.Image1 = "/Content/images/userfiles/" + path;
                    
                }
                else
                {
                    model.Product.Image1 = "/Content/images/front/noimage.jpeg";
                }

                if (prd2 != null)
                {
                    //prd1.SaveAs(Server.MapPath("/Content/images/userfiles/") + item.FileName);
                    Random random = new Random();
                    int rand = random.Next(1000, 99999999);
                    string path = Utility.SetPagePlug(model.Product.Name) + "_" + rand + Path.GetExtension(prd2.FileName);
                    
                    prd2.SaveAs(Server.MapPath("/Content/images/userfiles/productbig/") + path);

                    new ImageHelper(210, 125).SaveThumbnail(prd2, "/Content/images/userfiles/", path);
                    model.Product.Image2 = "/Content/images/userfiles/" + path;
                }
                else
                {
                    model.Product.Image2 = "/Content/images/front/noimage.jpeg";
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


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditProduct(ProductAddModel model, IEnumerable<HttpPostedFileBase> attachments, HttpPostedFileBase prd1, HttpPostedFileBase prd2,int hdProductId)
        {
            try
            {
                model.Product.PageSlug = Utility.SetPagePlug(model.Product.Name);
                model.Product.ProductId = hdProductId;
            //    model.Product.ProductGroupId = 1;

                if (prd1 != null)
                {
                    //prd1.SaveAs(Server.MapPath("/Content/images/userfiles/") + item.FileName);
                    Random random = new Random();
                    int rand = random.Next(1000, 99999999);
                    string path = Utility.SetPagePlug(model.Product.Name) + "_" + rand + Path.GetExtension(prd1.FileName);

                    prd1.SaveAs(Server.MapPath("/Content/images/userfiles/productbig/") + path);                    

                    new ImageHelper(210, 125).SaveThumbnail(prd1, "/Content/images/userfiles/", path);
                    model.Product.Image1 = "/Content/images/userfiles/" + path;
                }
                else
                {
                        //model.Product.Image1 = "/Content/images/front/noimage.jpeg";
                }

                if (prd2 != null)
                {
                    //prd1.SaveAs(Server.MapPath("/Content/images/userfiles/") + item.FileName);
                    Random random = new Random();
                    int rand = random.Next(1000, 99999999);
                    string path = Utility.SetPagePlug(model.Product.Name) + "_" + rand + Path.GetExtension(prd2.FileName);

                    prd2.SaveAs(Server.MapPath("/Content/images/userfiles/productbig/") + path);                    
                    
                    new ImageHelper(210, 125).SaveThumbnail(prd2, "/Content/images/userfiles/", path);
                    model.Product.Image2 = "/Content/images/userfiles/" + path;
                }
                else
                {
                       //model.Product.Image2 = "/Content/images/front/noimage.jpeg";
                }

                ProductManager.EditProduct(model.Product);

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
                        p.ItemId = Convert.ToInt32(RouteData.Values["id"]);
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



            var photos = PhotoManager.GetList(11, Convert.ToInt32(RouteData.Values["id"]));
            ViewBag.Photos = photos;

            web.Areas.Admin.Models.VMProductGroupModel grouplist = new Models.VMProductGroupModel();
            grouplist.ProductGroup = ProductManager.GetProductGroupList("tr");
            
            model.VMProductGroupModel = grouplist;
           

            return View(model);
        }



        public ActionResult SaveDetail(string code, string malzeme, string birim, string ebat, string agirlik, string ton, string fiyat, string renk, string prId)
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
                    //pd.ColorName = renkadi;

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

        public ActionResult GetDetail(int id)
        {
            using (MainContext db = new MainContext())
            {
                   ViewBag.Details = db.ProductDetail.Where(x => x.ProductId == id).ToList();
                   return PartialView("~/Areas/Admin/Views/Product/_detailtable1.cshtml", ViewBag.Details);
             }
        }


        public ActionResult Index(int? groupId)
        {
            using (MainContext db = new MainContext())
            {
                VMProductGroupModel vm = new VMProductGroupModel();
                vm.ProductGroup = ProductManager.GetProductGroupList("tr");
                if (groupId==null)
                    vm.Products = db.Product.Where(x => x.Deleted==false).ToList();
                else vm.Products = db.Product.Where(x => x.Deleted == false && x.TopProductGroupId == groupId).OrderBy(d=>d.SortNumber).ToList();
                return View(vm);
            }
          
        }

        public void SaveCategory(int id, int prdId)
        {
            using (MainContext db = new MainContext())
            {
                int topid = id;
                ProductGroup prodgroup;
                prodgroup = ProductManager.GetTopGroupById(id);
                while (prodgroup.TopProductId != 1)
                {
                    prodgroup = ProductManager.GetTopGroupById(prodgroup.TopProductId);
                    topid = prodgroup.ProductGroupId;
                }

                Product prd = db.Product.Where(x => x.ProductId == prdId).SingleOrDefault();
                if (prd != null)
                {
                    prd.TopProductGroupId = topid;
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

        [AllowAnonymous]
        public JsonResult DeleteDetail(int id)
        {
            using (MainContext db = new MainContext())
            {
                try
                {
                    var record = db.ProductDetail.FirstOrDefault(d => d.DetailId == id);
                    db.ProductDetail.Remove(record);
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

        public JsonResult DeletePhoto(int id)
        {
            bool isdelete = PhotoManager.Delete(id);
            return Json(isdelete);
        }


        public JsonResult EditStatus(int id)
        {
            string NowState;
            using (MainContext db = new MainContext())
            {
                var list = db.Product.SingleOrDefault(d => d.ProductId == id);
                try
                {

                    if (list != null)
                    {
                        list.Online = list.Online == true ? false : true;
                        db.SaveChanges();

                    }
                    return Json(list.Online);

                }
                catch (Exception)
                {
                    return Json(list.Online);
                }
            }
        }
    }
}
