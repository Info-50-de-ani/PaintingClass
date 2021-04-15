﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;
using System.Text.Json;
using System.Windows.Markup;
using PaintingClass.Tabs;
using PaintingClassCommon;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace PaintingClass.Networking
{
    public class RoomManager
    {
        public static bool CertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
            // TODO
            return true;
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
			ws = new WebSocket($"{Constants.urlWebSocket}/room/{userData.roomId}?name={userData.name}&clientID={userData.clientID}&profToken={userData.profToken}");
            ws.SslConfiguration.ServerCertificateValidationCallback = CertificateValidation;
            ws.OnError += (sender, e) => { MessageBox.Show(e.Message); };
			ws.OnOpen += (sender,e) => { ws.Send("0"); };
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
                    WhiteboardMessage wm = JsonSerializer.Deserialize<WhiteboardMessage>(p.msg);
                    if (wm.clientId == MainWindow.userData.clientID) break;
                    if (userList.TryGetValue(wm.clientId,out NetworkUser user))
                    {
                        MessageUtils.ApplyWhiteboardMessage(wm, user.collection);
                    }
                    break;

                case PacketType.UserListMessage:
                    var msg = JsonSerializer.Deserialize<UserListMessage>(p.msg);

                    foreach (var item in msg.list)
                    {
                        if (item.id == MainWindow.userData.clientID) continue;
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
