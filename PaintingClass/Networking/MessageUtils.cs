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
using System.IO;
using System.Windows.Media.Imaging;

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

        [Serializable]
        public class WBImage
        {
            /// <summary>
            /// imaginea in jpg
            /// </summary>
            public byte[] data { get; set; }

            /// <summary>
            /// Drawing
            /// </summary>
            public string serializedDrawing { get; set; }

            // pt serializare
            public WBImage() { }

			public WBImage(ImageDrawing drawing)
			{
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)drawing.ImageSource));
                using MemoryStream ms = new MemoryStream();
                encoder.Save(ms);
                data = ms.ToArray();
                serializedDrawing = XamlWriter.Save(drawing);
            }

            public ImageDrawing Deserialize()
			{
                var drawing = (ImageDrawing)XamlReader.Parse(serializedDrawing);
                MemoryStream ms = new MemoryStream(data);
                BitmapImage bitmapImage = new BitmapImage();
                ms.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
                drawing.ImageSource = bitmapImage;
                return drawing;
			}
        }


        public static void SendNewWBImage(WBImage image,int index)
		{
            MainWindow.instance.roomManager?.SendWVBtem(new WBItemMessage
            {
                clientID = MainWindow.userData.clientID,
                contentIndex = index,
                type = WBItemMessage.ContentType.drawing,
                op = WBItemMessage.Operation.add,
                content = JsonSerializer.Serialize(image)
            });
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
