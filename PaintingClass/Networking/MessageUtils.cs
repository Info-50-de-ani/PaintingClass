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
using System.Windows.Controls;

namespace PaintingClass.Networking
{
    public static class MessageUtils
    {
        /// <summary>
        /// Creaza un WBItemMessage care este trimis
        /// </summary>
        public static void SendNewDrawing(Drawing drawing, int index)
        {
            MainWindow.instance.roomManager?.SendWVBtem(new WBItemMessage
            {
                clientID = MainWindow.userData.clientID,
                contentIndex=index,
                type=WBItemMessage.ContentType.drawing,
                op=WBItemMessage.Operation.add,
                content = XamlWriter.Save(drawing)
            });
        }
    }
}
