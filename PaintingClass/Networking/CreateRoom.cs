using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace PaintingClass.Networking
{
    /// <summary>
    /// Folosita de pagina de logare pentru a cere crearea unui nou Room
    /// </summary>
    public static class CreateRoom
    {
        const string endpoint = "/createRoom";
        public static void SendRequest(int profToken, Action<int> onCompletion)
        {
            //daca este 0 inseamna ca ceva rau sa intamplat
            int result = 0;
            WebSocket ws = new WebSocket(Constants.url + endpoint + $"?profToken={profToken}");
            ws.OnMessage += (sender, args) =>
            {
                result = Convert.ToInt32(args.Data);
            };
            ws.OnError += (sender, args) =>
            {
                System.Diagnostics.Trace.WriteLine("createRoom websocket error with code: " + (args.Exception as WebSocketException).Code.ToString());
            };
            ws.OnClose += (sender,args) =>
            {
                onCompletion?.Invoke(result);
            };
            ws.ConnectAsync();
        }
    }
}
