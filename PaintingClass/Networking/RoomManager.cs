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

        public static Dictionary<int,NetworkUser> userList = new();


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

                    //updatam lista de utilizatori
                    foreach (var nu in userList) nu.Value.isConnected = false;
                    for (int i=0;i<msg.idList.Length;i++)
                    {
                        if (msg.idList[i] == MainWindow.userData.clientID) continue;
                        if (!userList.TryGetValue(msg.idList[i], out var networkUser))
                            networkUser.isConnected = true;
                        else
                            userList.Add(msg.idList[i],new NetworkUser { clientId = msg.idList[i], name = msg.nameList[i],isConnected=true });
                    }
                    break;

                case PacketType.ShareRequestMessage:
                    ShareRequestMessage sm = JsonSerializer.Deserialize<ShareRequestMessage>(p.msg);
                    if (sm.clientId == MainWindow.userData.clientID)
                    {
                        //TODO:
                    }
                    else
                    {
                        userList[sm.clientId].isShared = sm.shared;
                    }
                    break;
                default:
                    break;
            }
        }
    }

}