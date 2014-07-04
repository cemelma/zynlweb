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

namespace web.Areas.Admin.Controllers
{
    [AuthenticateUser]
    public class Product : Controller
    {
        public ActionResult AddProduct()
        {
            string lang = FillLanguagesList();
            var productgroup = ProductManager.GetProductGroupList(lang);
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(string txtname, int topProductGroupId)
        {
            string lang = FillLanguagesList();
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


        string FillLanguagesList()
        {
            string lang = "";
            if (RouteData.Values["lang"] == null)
                lang = "tr";
            else lang = RouteData.Values["lang"].ToString();

            var languages = LanguageManager.GetLanguages();
            var list = new SelectList(languages, "Culture", "Language", lang);
            ViewBag.LanguageList = list;
            return lang;
        }
    }
}
