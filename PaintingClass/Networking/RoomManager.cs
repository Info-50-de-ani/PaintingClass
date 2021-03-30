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
using PaintingClassCommon;

namespace PaintingClass.Networking
{
    public class RoomManager
    {
        //tine informatii legate de alt utilizator
        public class NetworkUser
        {
            public int clientId;
            public string name;
            public bool isConnected;
            bool _isShared;
            public bool isShared
            {
                get => _isShared;
                set
                {
                    if (_isShared == value)
                        return;
                    if (value==true)
                    {

                    }
                    else
                    {

                    }
                    _isShared = value;
                }
            }
            
        }

        public WebSocket ws;

        public Dictionary<int,NetworkUser> userList = new();
        public Action onUserListUpdate;


        public void PackAndSend<T>(PacketType type, T msg)
        {
            string msgSerialized = JsonSerializer.Serialize<T>(msg);
            ws.Send(Packet.Pack(type,msgSerialized));
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
                    var msg = JsonSerializer.Deserialize<UserListMessage>(p.msg);

                    foreach (var item in msg.list)
                    {
                        NetworkUser nu;
                        if (!userList.TryGetValue(item.id, out nu ))
                        {
                            nu = new NetworkUser { clientId = item.id };
                            userList.Add(nu.clientId, nu);
                        }
                        nu.name = item.name;
                        nu.isShared = item.isShared;
                        nu.isConnected = item.isConnected;
                    }
                    onUserListUpdate?.Invoke();
                    break;
                     
                default:
                    break;
            }
        }
    }
}
