using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Challenge.Models;

namespace Challenge.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HttpCookie sessionCookie = HttpContext.Request.Cookies.Get("session");
            if (sessionCookie == null) 
                return Redirect("/Auth/Login");

            User user = Challenge.Models.User.GetUser(sessionCookie.Value);

            if (user == null) 
                return Redirect("/Auth/Login");

            List<Website> websites = Website.GetWebsites(user.Uuid);

            for (int i=0; i < 3 && i < websites.Count; i++)
            {
                ViewData["Website" + (i+1).ToString()] = websites[i].Url;
                ViewData["Website" + (i+1).ToString() + "Status"] = websites[i].Status;
            }

            if (System.IO.File.Exists(Server.MapPath("~/Content/images/" + user.Uuid + ".png")))
                ViewData["Avatar"] = "/Content/images/" + user.Uuid + ".png";
            else
                ViewData["Avatar"] = "/Content/images/" + user.Uuid + ".jpg";

            ViewData["Username"] = user.Username;
            ViewData["Uuid"] = user.Uuid;

            return View();
        }
        
        [HttpPost]
        public ActionResult AddWebsite(string url)
        {
            HttpCookie sessionCookie = HttpContext.Request.Cookies.Get("session");
            if (sessionCookie == null) 
                return Redirect("/Auth/Login");
            User user = Challenge.Models.User.GetUser(sessionCookie.Value);
            if (user == null) 
                return Redirect("/Auth/Login");

            List<Website> websites = Website.GetWebsites(user.Uuid);

            Website site = new Website(url);
            websites.Insert(0, site);

            while (websites.Count > 3)
            {
                Website itemToRemove = websites[3];
                websites.Remove(itemToRemove);
            }

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(Server.MapPath("~/App_Data/UsersData/") + user.Uuid, FileMode.Open);

            formatter.Serialize(fs, websites);

            fs.Close();

            return Redirect("/Home/Index");
        }
    }
}