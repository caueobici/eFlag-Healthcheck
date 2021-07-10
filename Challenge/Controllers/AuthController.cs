using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Challenge.Models;

namespace Challenge.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public RedirectResult Register(string username, string password, HttpPostedFileBase file)
        {
            
            string uuid = Guid.NewGuid().ToString();

            if (file == null)
            {
                return Redirect("/Auth/Register?info=Nao+foi+enviado+nenhuma+foto");
            }

            string profilePicturePath = Server.MapPath("~/Content/images/") + uuid;

            if (file.FileName.EndsWith(".png")) 
                profilePicturePath += ".png";
            else if (file.FileName.EndsWith(".jpg")) 
                profilePicturePath += ".jpg";
            else return 
                    Redirect("/Auth/Register?info=apenas+png+e+jpg");

            file.SaveAs(profilePicturePath);

            User user = new User(username, password, profilePicturePath, uuid);
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream fs = new FileStream(Server.MapPath("~/App_Data/Users/") + uuid, FileMode.Create);
            formatter.Serialize(fs, user);
            fs.Close();

            FileStream fs2 = new FileStream(Server.MapPath("~/App_Data/UsersData/") + uuid, FileMode.Create);
            List<Website> websites = new List<Website>();
            formatter.Serialize(fs2, websites);

            fs2.Close();

            return Redirect("/Auth/Login?info=Conta+criada+com+sucesso&uuid=" + uuid);
        }

        public ActionResult Login()
        {
            return View();
        }
       
        [HttpPost]
        public RedirectResult Login(string uuid, string password)
        {
            User user = Challenge.Models.User.GetUser(uuid);

            if (user == null)
            {
                return Redirect("/Auth/Login?info=Usuario+ou+senha+incorretos");
            }

            if (!user.CheckPassword(password))
            {
                return Redirect("/Auth/Login?info=Usuario+ou+senha+incorretos");
            }

            HttpCookie sessionCookie = new HttpCookie("session");
            sessionCookie.Value = uuid;
            sessionCookie.Expires = DateTime.Now.AddHours(2);
            Response.Cookies.Add(sessionCookie);

            return Redirect("/Home/Index");
        }
    }
}
