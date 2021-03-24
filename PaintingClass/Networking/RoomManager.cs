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
        public WebSocket ws;
        public UserListMessage userList;
        
        //public static Dictionary<string, UserData> whiteboardCollections = new Dictionary<string, UserData>();

        public void SendWhiteboardMessage(WhiteboardMessage msg)
        {
            string msgSerialized = JsonSerializer.Serialize(msg, typeof(WhiteboardMessage));
            ws.Send(Packet.Pack(PacketType.WhiteboardMessage,msgSerialized));
        }

        public RoomManager(UserData userData)
        {
            ws = new WebSocket($"{Constants.url}/room/{userData.roomId}?name={userData.name}&clientID={userData.clientID}&profToken={userData.profToken}");
            ws.OnMessage += OnMessage;
            ws.Connect();

        }

        void OnMessage(object sender, MessageEventArgs e)
        {
            Packet p = Packet.Unpack(e.Data);

            switch (p.type)
            {
                case PacketType.none:
                    throw new Exception("PacketType nu ar trb sa fie 0");
                case PacketType.WhiteboardMessage:
                    //todo:
                    break;
                case PacketType.UserListMessage:
                    userList = JsonSerializer.Deserialize<UserListMessage>(p.msg);
                    break;
                default:
                    break;
            }
        }
    }
}
