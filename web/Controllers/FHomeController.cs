using System.Linq;
using System.Web.Mvc;
using web.Models;
using BLL.NewsBL;
using BLL.ReferenceBL;
using BLL.DocumentsBL;
using BLL.PhotoBL;
using BLL.SectorGroupBL;
using BLL.ProductBL;
using BLL.ServiceGroupBL;
using BLL.ContactBL;
using System.Collections.Generic;
using DAL.Context;
using DAL.Entities;
using BLL.ServiceBL;

namespace web.Controllers
{
    public class slider
    {
        public string image { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string thumb { get; set; }
    }

    public class FHomeController : Controller
    {
        public ActionResult Index()
        {
            HomePageWrapperModel model = new HomePageWrapperModel();
            model.photos = PhotoManager.GetListForFront("tr",0);
            model.news = NewsManager.GetNewsListForFront("tr");
            model.servicegroups = ServiceManager.GetServiceList();
            model.references = ReferenceManager.GetReferenceListForFront("tr");
  
            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult GetAddress()
        {
            Contact cont=ContactManager.GetContact();
           ViewBag.Services = ServiceManager.GetServiceListForFront("tr").Take(3);
            return PartialView("Partial/_footeraddress",cont);
        }

        [ChildActionOnly]
        public PartialViewResult GetTopMenu()
        {
            ViewBag.Services = ServiceManager.GetServiceListForFront("tr");
            return PartialView("Partial/_topmenu");
        }



        public JsonResult GetImages()
        {
            var photos = PhotoManager.GetListForFront("tr", 0);
            
            var slider = new List<slider>();    

            foreach (var item in photos)
            {
                slider s = new slider();
                s.image = item.Path;
                s.title = item.Title;
                s.url = item.Link;
                s.thumb = "";
                slider.Add(s);
            }

            return Json(slider
                ,
                JsonRequestBehavior.AllowGet
                );
        }
        public ActionResult ChangeCulture(string lang,string returnUrl)
        {
            Session["culture"] = lang;
            if(lang=="en")
                return Redirect("/en/homepage");
            return Redirect("/tr/anasayfa");
        }
    }
}
