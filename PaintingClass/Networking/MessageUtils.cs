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
using WpfMath.Controls;

namespace PaintingClass.Networking
{
    public static class MessageUtils
    {
        /// <summary>
        /// Creaza un WBItemMessage care este trimis si inregistreaza pt undo
        /// </summary>
        public static void SendNewDrawing(Drawing drawing, int index, bool pushToUndoBuffer=true)
        {
            WBItemMessage msg = new()
            {
                clientID = MainWindow.userData.clientID,
                contentIndex=index,
                type=WBItemMessage.ContentType.drawing,
                op=WBItemMessage.Operation.add,
                content = XamlWriter.Save(drawing)
            };
            if (pushToUndoBuffer)
                MainWindow.instance.myWhiteboard.PushToUndoBuffer(msg);
            MainWindow.instance.roomManager?.SendWVBtem(msg);
        }

        /// <summary>
        /// Coordonatele sunt in coordonatele tablei 0-100,0-100
        /// </summary>
        [Serializable]
        public class UserControlWBMessage
        {
            #region Props
            public int uniqueControlId { get; set; }
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
            public UserControlWBMessage(Point pos, FrameworkElement control,int uniqueControlId)
            {
                this.uniqueControlId = uniqueControlId;
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
                    var tb = (TextBox)control;

                    /// daca este formula 
                    if(control.Name == nameof(FormulaControl))
				    {
                        FormulaControl fc = new FormulaControl { Formula = tb.Text, Tag = tb.Tag };
                        output = fc;
                    }
                    else
					{
                        output = new TextBox { IsReadOnly = true, Text = tb.Text , Tag = tb.Tag, Background = tb.Background, TextWrapping = tb.TextWrapping, FontSize = tb.FontSize };
                        output.Width = control.Width;
                        output.Height = control.Height;
					}
                }
                output.RenderTransform = new ScaleTransform(width / control.Width, height / control.Height);
                Canvas.SetLeft(output, X);
                Canvas.SetTop(output, Y);
                return output;
            }
        }

        public static void SendNewUserControl(UserControlWBMessage userControl, int index)
        {
            MainWindow.instance.roomManager?.SendWVBtem(new WBItemMessage
            {
                clientID = MainWindow.userData.clientID,
                contentIndex = index,
                type = WBItemMessage.ContentType.control,
                op = WBItemMessage.Operation.add,
                content = JsonSerializer.Serialize(userControl)
            });
        }

        public static void EditUserControl(UserControlWBMessage userControl, int index)
		{
            MainWindow.instance.roomManager?.SendWVBtem(new WBItemMessage
            {
                clientID = MainWindow.userData.clientID,
                contentIndex = index,
                type = WBItemMessage.ContentType.control,
                op = WBItemMessage.Operation.edit,
                content = JsonSerializer.Serialize(userControl)
            });
        }

        public static void DeleteUserControl(int contentIndex,int uniqueControlId)
		{
            MainWindow.instance.roomManager?.SendWVBtem(new WBItemMessage
            {
                clientID = MainWindow.userData.clientID,
                contentIndex = contentIndex,
                type = WBItemMessage.ContentType.control,
                op = WBItemMessage.Operation.delete,
                content = JsonSerializer.Serialize(new UserControlWBMessage { uniqueControlId = uniqueControlId })
            }); 
        }

        [Serializable]
        public class WBImage
        {
            /// <summary>
            /// imaginea in jpg
            /// </summary>
            public byte[] data { get; set; }
            public double posX { get; set; }
            public double posY { get; set; }
            public double rectHeight { get; set; }
            public double rectWidth { get; set; }
            // pt serializare
            public WBImage() { }

			public WBImage(ImageDrawing drawing)
			{
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)drawing.ImageSource));
                using MemoryStream ms = new MemoryStream();
                encoder.Save(ms);
                data = ms.ToArray();
                posX = drawing.Rect.TopLeft.X;
                posY = drawing.Rect.TopLeft.Y;
                rectHeight = drawing.Rect.Height;
                rectWidth = drawing.Rect.Width;
            }

            public ImageDrawing Deserialize()
			{
                MemoryStream ms = new MemoryStream(data);
                BitmapImage bitmapImage = new BitmapImage();
                ms.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
                ImageDrawing drawing = new ImageDrawing { Rect = new Rect(new Point(posX,posY),new Size(rectWidth,rectHeight))};
                drawing.ImageSource = bitmapImage;
                return drawing;
			}
        }


        public static void SendNewWBImage(WBImage image,int index, bool pushToUndoBuffer = true)
		{
            WBItemMessage msg = new()
            {
                clientID = MainWindow.userData.clientID,
                contentIndex = index,
                type = WBItemMessage.ContentType.drawing,
                op = WBItemMessage.Operation.add,
                content = JsonSerializer.Serialize(image)
            };
            if (pushToUndoBuffer)
                MainWindow.instance.myWhiteboard.PushToUndoBuffer(msg);
            MainWindow.instance.roomManager?.SendWVBtem(msg);
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
