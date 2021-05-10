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
        public static readonly string urlWebSocket = $"wss://79.114.90.29:{portWebSocket}";
        public static readonly string urlHttp = $"http://79.114.90.29:{portHttp}";
        public const int portWebSocket = 32281;
        public const int portHttp = 32221;
        public const string customProtocol = "PaintingClass";
    }
}
