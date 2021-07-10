using System;
using System.Web;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;


namespace Challenge.Models
{
    [Serializable]
    public class User
    {
        public string Uuid { get; set; }
        public string Username { get; set; }
        private byte[] Password { get; set; }
        public string ProfilePicture { get; set; }
        public string[] Websites { get; set; }

        public User(string username, string password, string ProfilePicturePath, string uuid)
        {
            this.ProfilePicture = ProfilePicturePath;
            this.Username = username;
            this.SetPassword(password);
            this.Uuid = uuid;
        }

        public bool CheckPassword(string password)
        {
            SHA256 Sha256 = SHA256.Create();
            byte[] hash = Sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            return hash.SequenceEqual(this.Password);
        }

        private void SetPassword(string password)
        {
            SHA256 Sha256 = SHA256.Create();
            this.Password = Sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public static User GetUser(string uuid)
        {

            string fileName = HttpContext.Current.Server.MapPath("~/App_Data/Users/" + uuid);
            if (!File.Exists(fileName)) 
                return null;

            FileStream fs = new FileStream(fileName, FileMode.Open);


            User user = null;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                user = (User)formatter.Deserialize(fs);
            }
            catch
            {
                user = null;
            }
            finally
            {
                fs.Close();
            }

            return user;
        }

    }
}