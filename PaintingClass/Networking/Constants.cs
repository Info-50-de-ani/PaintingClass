using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintingClass.Networking
{
    public static class Constants
    {
        //cui nui plac url-urile hardcodate?
        public static readonly string urlWebSocket = $"wss://109.102.218.63:{portWebSocket}";
        public static readonly string urlHttp = $"http://109.102.218.63:{portHttp}";
        public const int portWebSocket = 32281;
        public const int portHttp = 32221;
        public const string customProtocol = "PaintingClassLauncher";
    }
}
