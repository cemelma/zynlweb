using BLL.ProductBL;
using BLL.ReferenceBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Controllers
{
    public class FProductController : Controller
    {
        public ActionResult Index(int id)
        {
            //web.Areas.Admin.Models.VMProductGroupModel grouplist = new web.Areas.Admin.Models.VMProductGroupModel();
            //grouplist.ProductGroup = ProductManager.GetProductGroupList("tr");
            int pId = Convert.ToInt32(RouteData.Values["id"]);
            ViewBag.ProductGroup = ProductManager.GetProductGroupItem(pId);
            ViewData["referanslar"] = ReferenceManager.GetReferenceListForFront("tr");
            return View();
        }

        public ActionResult Prices()
        {
            if (RouteData.Values["id"] != null)
            {
                int pId = Convert.ToInt32(RouteData.Values["id"]);
                ViewBag.ProductGroup = ProductManager.GetProductGroupItem(pId);
            }
            return View();
        }

    }
}
