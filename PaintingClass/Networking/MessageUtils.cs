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
		public class UserControlWBMessage
		{
			#region Props
			public double X { get; set; }
            public double Y { get; set; }
            public double width { get; set; }
            public double height { get; set; }

            /// <summary>
            /// Contine UserControlul serializat
            /// </summary>
            public string serializedUserControl { get; set; }
            #endregion

            public UserControlWBMessage()
			{

			}

            /// <summary>
            /// Este utilizat cand doresti sa trimiti la server un control
            /// </summary>
            /// <param name="pos">poz in coord tabla</param>
            /// <param name="size">marimea in coord tabla</param>
            /// <param name="control">controlul de serializat</param>
			public UserControlWBMessage(Point pos, FrameworkElement control)
			{
                X = pos.X;
                Y = pos.Y;
                width = control.Width / MainWindow.instance.myWhiteboard.myWhiteboardCanvas.ActualWidth * Whiteboard.sizeX;
                height = control.Height / MainWindow.instance.myWhiteboard.myWhiteboardCanvas.ActualHeight * Whiteboard.sizeY;
                serializedUserControl = XamlWriter.Save(control);
			}

            public FrameworkElement Deserialize(Whiteboard wb)
			{
                // obtinem usercontrolul trimis 
                FrameworkElement control = (FrameworkElement)XamlReader.Parse(serializedUserControl);
                FrameworkElement output = null;

                // daca controlul este un TextBox il facem readonly 
                if (control is TextBox)
				{
                    output = new TextBox() { IsReadOnly = true, Text = ((TextBox)control).Text};
				}

                output.Width = control.Width;
                output.Height = control.Height;
                output.RenderTransform = new ScaleTransform(width/ control.Width, height/control.Height) ;
                Canvas.SetLeft(output, X);
                Canvas.SetTop(output, Y);
                return output;
			}
        }

        public static void SendNewUserControl(UserControlWBMessage userControl,int index)
		{
            MainWindow.instance.roomManager?.SendWVBtem(new WBItemMessage
            {
                clientID = MainWindow.userData.clientID,
                contentIndex = index,
                type = WBItemMessage.ContentType.userControl,
                op = WBItemMessage.Operation.add,
                content = JsonSerializer.Serialize(userControl)
            });
		}
    }
}
