using BLL.HRBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Controllers
{
    public class FHumanResourcesController : Controller
    {
        //
        // GET: /FHumanResources/

        public ActionResult Index()
        {
            var model=HumanResourceManager.GetHumanResourcePositionList();
            return View(model);
        }

    }
}
