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
using System.Windows;

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

        /// <summary>
        /// Coordonatele sunt in coordonatele tablei 0-100,0-100
        /// </summary>
        [Serializable]
		public class ControlWithPosition
		{
			public double X { get; set; }
            public double Y { get; set; }

            /// <summary>
            /// Contine control-ul serializat
            /// </summary>
            public string serializedControl { get; set; }
        }

        public static void SendNewUserControl(Control control, Point pos ,int index)
		{
            ControlWithPosition cwp = new()
            {
                X = pos.X,
                Y = pos.Y,
                serializedControl = XamlWriter.Save(control)
            };

            MainWindow.instance.roomManager?.SendWVBtem(new WBItemMessage
            {
                clientID = MainWindow.userData.clientID,
                contentIndex = index,
                type = WBItemMessage.ContentType.control,
                op = WBItemMessage.Operation.add,
                content = JsonSerializer.Serialize(cwp)
            });
		}

        public static void SendClearAll()
        {
            MainWindow.instance.roomManager?.SendWVBtem(new WBItemMessage
            {
                clientID = MainWindow.userData.clientID,
                type = WBItemMessage.ContentType.clearAll
            });
        }
    }
}
