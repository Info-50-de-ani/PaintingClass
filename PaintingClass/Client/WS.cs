using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Net;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Windows;
namespace PaintingClass.Client
{
    public static class WS
    {
        private static Dictionary<int, string> localUserStash = new Dictionary<int, string>();
        private static Thread T_InRoom = null;
        private static bool? _isHost = null;
        private static string _URL;
        private static string _name;
        public static WebSocketSharp.WebSocket ws;
        //connect to server as host
        public static void InitHost(string URL, string name, EventHandler WSOpenEventHander = null)
        {
            _URL = URL;
            _name = name;
            _isHost = true;
            localUserStash.Add(0, WS._name);
            ws = new WebSocketSharp.WebSocket($"{URL}?host={_isHost}");
            WS.WSEventSubscribtions();
            // event handler extra pt chestii cum ar fi display roomCode
            if(WSOpenEventHander!=null)
            {
                Host.ExtraOnOpenEventHandler = WSOpenEventHander;
            }

            ws.Connect();
        }
        //connect to server as client
        public static void InitClient(string URL, string name, int roomCode)
        {
            _URL = URL;
            _name = name;
            Debug.Assert(roomCode != null);
            _isHost = false;
            ws = new WebSocketSharp.WebSocket($"{URL}?host={_isHost}&room={roomCode}");
            WS.WSEventSubscribtions();
            ws.Connect();
        }
        public static void Close()
        {
            while (T_InRoom == null || !T_InRoom.IsAlive) { }
            T_InRoom.Join();
        }
        public static void WSEventSubscribtions()
        {
            Debug.Assert(_isHost != null);
            if ((bool)_isHost)
                ws.OnMessage += (sender, e) =>
                {
                    if (e.Data.Contains("ws://"))
                    {
                        T_InRoom = new Thread(() => Host.InRoom(e.Data + $"?name={_name}"));
                        T_InRoom.Start();
                        ws.Close();
                    }
                    else
                        Console.WriteLine(e.Data); //delete
                };
            else
                ws.OnMessage += (sender, e) =>
                {
                    if (e.Data.Contains("ws://"))
                    {
                        T_InRoom = new Thread(() => Client.InRoom(e.Data + $"?name={_name}&room={Client.roomCode}"));
                        T_InRoom.Start();
                        ws.Close();
                    }
                    else
                        Console.WriteLine(e.Data); //delete
                };
            //ws.OnOpen += (sender, e) => { ws.Send("0"); };
            ws.OnError += (sender, e) => MessageBox.Show(e.Message); 
        } 
        public static class Host
        {
            public static EventHandler ExtraOnOpenEventHandler = null;
            public static int roomCode;
            static public WebSocket ws;
            public static void InRoom(string URL)
            {
                Debug.Assert(int.TryParse(URL.Substring(URL.IndexOf("room") + 5, 7), out roomCode));
                ws = new WebSocket(URL);
                EventSubscribtions();
                ws.Connect();
                string msg = "da";
                while (msg != "exit")
                {
                    msg = Console.ReadLine(); //delete
                    ws.Send(msg);
                }
            }
            public static void EventSubscribtions()
            {
                ws.OnOpen += (sender, e) =>
                {
                    //MessageBox.Show($"Connected to room {roomCode}");
                };
                if (ExtraOnOpenEventHandler != null)
                {
                    ws.OnOpen += ExtraOnOpenEventHandler;
                }
                ws.OnMessage += (sender, e) =>
                {
                    Console.WriteLine(e.Data); //delete
                };
                ws.OnError += (sender, e) => Console.WriteLine(e.Message); //delete
            }
        }
        public static class Client
        {
            public static int roomCode;
            public static WebSocket ws;
            public static void InRoom(string URL)
            {
                ws = new WebSocket(URL);
                Debug.Assert(int.TryParse(URL.Substring(URL.IndexOf("room") + 5, 7), out roomCode));
                Client.ClientEnventSubscribtions();
                ws.Connect();
                string msg = "da";
                while (msg != "exit")
                {
                    msg = Console.ReadLine(); //delete
                    ws.Send(msg); 
                }
            }
            public static void ClientEnventSubscribtions()
            {
                ws.OnOpen += (sender, e) =>
                {
                    MessageBox.Show($"Connected to room {roomCode}");
                    ws.Send("0");
                };
                ws.OnMessage += (sender, e) =>
                {
                    if (e.Data == "EXIT")
                        ws.Close();
                    else if (e.Data.Substring(0, 3) == "RCV")
                    {
                        Console.WriteLine(e.Data);//delete
                    }
                    else if (e.Data.Substring(0, 3) == ("CON"))
                    {
                        string[] dat = e.Data.Split();
                        localUserStash.Add(int.Parse(dat[1]), dat[2]);
                    }
                    else
                        Console.WriteLine(e.Data);//delete
                };
                ws.OnError += (sender, e) => Console.WriteLine(e.Message);//delete
            }
        }
    }
}
