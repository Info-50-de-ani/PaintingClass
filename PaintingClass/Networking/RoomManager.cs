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

namespace PaintingClass.Networking
{
    public class RoomManager
    {
        [Serializable]
        public class Message
        {
            public enum MessageType
            {
                Drawing,Action
            };

            public int clientId { get; set; }
            public MessageType type { get; set; }
            public string content { get; set; }

            public static Message SerialzieDrawing(Drawing drawing)
            {
                Message msg = new Message { clientId = MainWindow.userData.clientID, type=MessageType.Drawing };
                msg.content = XamlWriter.Save(drawing);
                return msg;
            }
            public static Message SerializeAction(string str)
            {
                Message msg = new Message { clientId = MainWindow.userData.clientID, type = MessageType.Action };
                msg.content = str;
                return msg;
            }

            public static object DeserializeContent(Message msg)
            {
                if (msg.type == MessageType.Drawing)
                {
                    return XamlReader.Parse(msg.content);
                }
                else 
                {
                    return msg.content;
                }
            }
        }

        public WebSocket ws;
        
        public static Dictionary<string, UserData> whiteboardCollections = new Dictionary<string, UserData>();

        public void SendMessage(Message msg)
        {
            string msgSerialized = JsonSerializer.Serialize(msg, typeof(Message));
            ws.Send(msgSerialized);
        }

        public RoomManager(UserData userData)
        {
            ws = new WebSocket($"{Constants.url}/room/{userData.roomId}?name={userData.name}&clientID={userData.clientID}&profToken={userData.profToken}");
            ws.OnMessage += (sender, e) =>
            {

            };
            ws.Connect();

        }
    }
}
