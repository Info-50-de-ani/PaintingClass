using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;
using System.Text.Json;
using System.Windows.Markup;
using PaintingClass.Tabs;
using System.Web;

namespace PaintingClass.Networking
{
    public enum PacketType
    {
        none=0, 
        WhiteboardMessage=1, 
        UserListMessage=2
    }
    [Serializable]
    public class Packet
    {

        public PacketType type { get; set; }
        public string msg { get; set; }

        public static Packet Unpack(string SerializedPacket)
        {
            return JsonSerializer.Deserialize<Packet>(SerializedPacket);
        }
        //evita creearea unui nou obiect deci e mai rapid
        //msg trebuie sa fie JSON
        public static string Pack(PacketType type, string msg)
        {
            string escapedMsg = HttpUtility.JavaScriptStringEncode(msg);
            return $"{{\"type\":{(int)type},\"msg\":\"{escapedMsg}\"}}";
        }
    }

    [Serializable]
    public class WhiteboardMessage
    {
        public enum ContentType
        {
            Drawing, Action
        };
       
        public int clientId { get; set; }
        public ContentType type { get; set; }
        public string content { get; set; }

        public static WhiteboardMessage SerialzieDrawing(Drawing drawing)
        {
            WhiteboardMessage msg = new WhiteboardMessage { clientId = MainWindow.userData.clientID, type = ContentType.Drawing };
            msg.content = XamlWriter.Save(drawing);
            return msg;
        }

        public static WhiteboardMessage SerializeAction(string str)
        {
            WhiteboardMessage msg = new WhiteboardMessage { clientId = MainWindow.userData.clientID, type = ContentType.Action };
            msg.content = str;
            return msg;
        }

        public static object DeserializeContent(WhiteboardMessage msg)
        {
            if (msg.type == ContentType.Drawing)
            {
                return XamlReader.Parse(msg.content);
            }
            else
            {
                return msg.content;
            }
        }
    }

    [Serializable]
    public class UserListMessage
    {
        public int[] idList { get; set;  }
        public string[] nameList { get; set;  }
    }
}
