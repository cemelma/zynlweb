using BLL.ProductBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Controllers
{
    public class FSiteMapController : Controller
    {
        public ActionResult Index()
        {
            web.Areas.Admin.Models.VMProductGroupModel grouplist = new web.Areas.Admin.Models.VMProductGroupModel();
            grouplist.ProductGroup = ProductManager.GetProductGroupList("tr");
            return View(grouplist);
        }

    }
}
