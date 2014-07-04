using BLL.HRBL;
using BLL.MailBL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace web.Controllers
{
    public class FHumanResourcesController : Controller
    {
        public ActionResult Index()
        {
            var model=HumanResourceManager.GetHumanResourcePositionList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase uploadfile)
        {
            Random random = new Random();
            int rand = random.Next(1000, 99999999);
            string targetFolder = Server.MapPath("~/Content/images/userfiles/humanresources");
            string targetPath = Path.Combine(targetFolder, rand + "_" + uploadfile.FileName);
            uploadfile.SaveAs(targetPath);

            var mset = MailManager.GetMailSettings();
            var msend = MailManager.GetMailUsersList(0);

            using (var client = new SmtpClient(mset.ServerHost, mset.Port))
            {
                client.EnableSsl = mset.Security;//true;//burası düzeltilecek
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential(mset.ServerMail, mset.Password);

                var mail = new MailMessage();
                mail.From = new MailAddress(mset.ServerMail, "Zeynel Yayla");

                mail.Attachments.Add(new Attachment(@targetFolder + "/" + rand + "_" + uploadfile.FileName));

                foreach (var item in msend)
                    mail.To.Add(item.MailAddress);
                mail.Subject = "Yeni CV";
                mail.IsBodyHtml = true;
                mail.Body = "<p> Merhaba \"Zeyne Yayla\" üzerinden Yeni bir CV gönderildi. <br> CV'yi mail ekinde bulabilirsiniz..</p>";

                if (mail.To.Count > 0) client.Send(mail);
            }
            TempData["sent"] = "true";

            var model = HumanResourceManager.GetHumanResourcePositionList();
            return View(model);
        }

    }
}
