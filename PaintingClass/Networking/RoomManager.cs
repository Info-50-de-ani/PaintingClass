using System;
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
using System.Diagnostics;

namespace PaintingClass.Networking
{
    //todo: UI pt Undo
    //todo: UI pt Clear All
    public class RoomManager
    {
        public static bool CertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
            // TODO ¯\_(ツ)_/¯
            return true;
		}

        WebSocket ws;

        public Dictionary<int,NetworkUser> userList = new();
        public Action onUserListUpdate;

        int wbItemIndex=0;//todo:could be replaced with whiteboardData.Count, maybe
        List<WBItemMessage> whiteboardData=new();

        public RoomManager(UserData userData)
        {
			ws = new WebSocket($"{Constants.urlWebSocket}/room/{userData.roomId}?name={userData.name}&clientID={userData.clientID}&profToken={userData.profToken}");
            ws.SslConfiguration.ServerCertificateValidationCallback = CertificateValidation;
            ws.OnError += (sender, e) => { MessageBox.Show(e.Message); };
            ws.OnMessage += OnMessage;
            ws.Connect();
        }
        #region Send stuff
        public void SendMessage(string msg)
        {
            ws.Send(msg);
        }
        /// <summary>
        /// Trimte un a WBItemMEssage. NU afecteaza nici o tabla!
        /// wbItemIndex este setat la valoarea corecta
        /// </summary>
        public void SendWVBtem(WBItemMessage msg)
        {
            msg.itemIndex = wbItemIndex;
            wbItemIndex++;
            whiteboardData.Add(msg);

            if (msg.type == WBItemMessage.ContentType.clearAll)
            {
                whiteboardData.Clear();
                wbItemIndex = 0;
            }

            SendMessage(Packet.Pack(PacketType.WBItemMessage, JsonSerializer.Serialize(msg)));
        }
        void SendSyncRequest(int clientID)
        {
            SendMessage(Packet.Pack(PacketType.SyncRequestMessage, JsonSerializer.Serialize( new SyncRequestMessage { clientID = clientID } ) ));
        }
        void ProcessWBItem(WBItemMessage msg, NetworkUser nu=null)
        {
            nu ??= userList[msg.clientID];

            //ne asiguram ca suntem sincronizati
            if (nu.wbItemIndex != msg.itemIndex)
            {
                Trace.WriteLine($"User {nu.clientId}'s whiteboard has desynced (bad wbItemIndex)");
                SendSyncRequest(nu.clientId);
                return;
            }
            nu.wbItemIndex++;

            if (msg.type == WBItemMessage.ContentType.clearAll)
            {
                nu.wbItemIndex = 0;
            }

            if (!App.Current.Dispatcher.Invoke( new Func<bool>(() => nu.whiteboard.ApplyWBItem(msg)) ))
            {
                Trace.WriteLine($"User {nu.clientId}'s whiteboard has desynced (apply function returned false)");
                SendSyncRequest(nu.clientId);
                return;
            }
        }
        #endregion

        void OnMessage(object sender, MessageEventArgs e)
        {
            Packet p = Packet.Unpack(e.Data);

            switch (p.type)
            {
                case PacketType.UserListMessage:
                    {
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
                            if (nu.wbItemIndex != item.wbItemIndex && (nu.isShared==true || MainWindow.userData.isTeacher==true) )
                                SendSyncRequest(item.id);
                        }
                        onUserListUpdate?.Invoke();
                        break;
                    }

                case PacketType.WBItemMessage:
                    {
                        WBItemMessage msg = JsonSerializer.Deserialize<WBItemMessage>(p.msg);
                        ProcessWBItem(msg);
                        break;
                    }
                case PacketType.WBCollectionMessage:
                    {
                        WBCollectionMessage wbColl = JsonSerializer.Deserialize<WBCollectionMessage>(p.msg);
                        NetworkUser nu = userList[wbColl.clientID];

                        if (wbColl.partial==false)
                        {
                            nu.wbItemIndex = 0;
                            App.Current.Dispatcher.Invoke(() => nu.whiteboard.ClearWhiteboard());
                        }

                        foreach (var item in wbColl.items)
                        {
                            ProcessWBItem(item, nu);
                        }
                        break;
                    }
                case PacketType.SyncRequestMessage:
                    {
                        var srmsg = JsonSerializer.Deserialize<SyncRequestMessage>(p.msg);
                        WBCollectionMessage coll = new()
                        {
                            clientID = srmsg.clientID,
                            partial = false,
                            items = whiteboardData.ToArray()
                        };
                        SendMessage( Packet.Pack( PacketType.WBCollectionMessage, JsonSerializer.Serialize(coll) ));
                        break;
                    }
                default:
                    break;
            }
        }
    }
}