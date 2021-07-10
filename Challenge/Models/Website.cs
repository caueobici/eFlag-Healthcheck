using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Management.Automation;
using System.Web;

namespace Challenge.Models
{
    [Serializable]
    public class Website : ISerializable
    {
        public string Url { get; set; }
        public string Status { get; set;  }

        public Website(string url) 
        {
            this.Url = EscapeUrl(url);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Url", Url);
        }

        public Website(SerializationInfo info, StreamingContext context)
        {
            Url = info.GetString("Url");
            try
            {
                PowerShell ps = PowerShell.Create();
                ps.AddScript("Invoke-WebRequest -UseBasicParsing -Uri \"" + Url + "\" | Select -exp StatusDescription");
                PSObject result = ps.Invoke()[0];

                Status = result.ToString();
            }
            catch
            {
                Status = "Erro";
            }
        }

        public static List<Website> GetWebsites(string uuid)
        {
            string fileName = HttpContext.Current.Server.MapPath("~/App_Data/UsersData/") + uuid;
            if (!File.Exists(fileName)) 
                return null;

            FileStream fs = new FileStream(fileName, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();

            List<Website> websites = (List<Website>)formatter.Deserialize(fs);

            fs.Close();
            return websites;
        }

        public static string EscapeUrl(string url) {

            try
            {
                Uri uri = new Uri(url);
                string port = uri.Port.ToString();

                if (port == "443" && uri.Scheme == "https") 
                    port = "";

                else if (port == "80" && uri.Scheme == "http") 
                    port = "";

                else 
                    port = ":" + port;


                url = "http://";
                if (uri.Scheme != "http") 
                    url = "https://";

                url += uri.Host + port + "/";

                return url;
            }
            catch ( UriFormatException )
            {
                return "https://invalid.url/";
            }

        }

    }
}