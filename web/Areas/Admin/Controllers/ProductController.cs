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

namespace web.Areas.Admin.Controllers
{
    [AuthenticateUser]
    public class ProductController : Controller
    {
        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(string txtname, int topProductGroupId)
        {
            string lang = "tr";
            if (ModelState.IsValid)
            {
                ProductGroup model = new ProductGroup();
                model.GroupName = txtname;
                model.Language = "tr";
                model.PageSlug = Utility.SetPagePlug(txtname);
                model.TopProductId = topProductGroupId;
                ViewBag.ProcessMessage = ProductManager.AddProductGroup(model);
                web.Areas.Admin.Models.VMProductGroupModel grouplist = new Models.VMProductGroupModel();
                grouplist.ProductGroup = ProductManager.GetProductGroupList(lang);
                return View(grouplist);
            }
            return View();
        }


        [HttpPost]
        public ActionResult AddProduct(Product model, IEnumerable<HttpPostedFileBase> attachments)
        {
            model.PageSlug = Utility.SetPagePlug(model.Name);
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

                    // Image img = Image.FromFile(Server.MapPath("/Content/images/userfiles/") + item.FileName);

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
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
       
    }
}
