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
using PaintingClassCommon;

namespace PaintingClass.Networking
{
    public static class MessageUtils
    {
        public static WhiteboardMessage SerialzieDrawing(Drawing drawing)
        {
            WhiteboardMessage msg = new WhiteboardMessage { clientId = MainWindow.userData.clientID, type = WhiteboardMessage.ContentType.Drawing };
            msg.content = XamlWriter.Save(drawing);
            return msg;
        }

        public static WhiteboardMessage SerializeAction(this string str)
        {
            WhiteboardMessage msg = new WhiteboardMessage { clientId = MainWindow.userData.clientID, type = WhiteboardMessage.ContentType.Action };
            msg.content = str;
            return msg;
        }

        public static object DeserializeContent(this WhiteboardMessage msg)
        {
            if (msg.type == WhiteboardMessage.ContentType.Drawing)
            {
                return XamlReader.Parse(msg.content);
            }
            else
            {
                return msg.content;
            }
        }
    }
}
