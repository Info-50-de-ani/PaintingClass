using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;
namespace PaintingClass.Networking
{
    public class RoomManager
    {
        WebSocket ws;
        public static Dictionary<string, UserData> whiteboardCollections = new Dictionary<string, UserData>();
        public RoomManager(UserData userData)
        {
            ws = new WebSocket($"{Constants.url}/room/{userData.roomId}?name={userData.name}&clientID={userData.clientID}&profToken={userData.profToken}");
            ws.OnMessage += (sender, e) =>
            {
                string[] data = e.Data.Split();
                if (data[2] != userData.name && data[1] == "BRC")
                {
                    //DrawingCollection dc = ((WhiteBoardCollectionSerialize)JsonSerializer.Deserialize(data[3], typeof(WhiteBoardCollectionSerialize))).collection;
                    foreach (var x in whiteboardCollections)
                    {
                        if (x.Key == userData.name)
                        {
                            var k = x.Value;
                            //k = dc;
                        }
                    }
                }
            };
            ws.Connect();

        }
    }
}
