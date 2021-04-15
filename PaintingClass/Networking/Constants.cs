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
        public readonly string urlWebSocket = $"wss://localhost:{portWebSocket}";
        public readonly string urlHttp = $"http://localhost:{portHttp}";
        public const int portWebSocket = 32281;
        public const int portHttp = 32221;
        public const string customProtocol = "PaintingClassLauncher";
    }
}
