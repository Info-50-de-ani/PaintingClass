using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaintingClass.Storage;

namespace PaintingClass.Client
{
    public static class User
    {
        public static SerializeUserInformation UserInformation { set; get; } = null;
        static User()
        {
            UpdateLocalData();
            if (UserInformation == null)
                UserInformation = new SerializeUserInformation();
        }
        static public void UpdateLocalData()
        {
            UserInformation = AppdataIO.Load<SerializeUserInformation>("UserData.xml") ;
        }
        static public void SaveLocalData()
        {
            AppdataIO.Save<SerializeUserInformation>("UserData.xml", UserInformation);
        }
        [Serializable]
        public class SerializeUserInformation
        {
            public bool? IsHost { set; get; } = null;
            public bool? IsLogged { set; get; } = null;
            public string? Name { set; get; } = null;
            public string? Email { get; set; } = null;
            public string? Password { set; get; } = null;
            public int? ClientToken { set; get; } = null;
        }

    }
}
