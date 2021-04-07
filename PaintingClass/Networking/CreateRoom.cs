﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using System.Threading;
using System.Windows;

namespace PaintingClass.Networking
{
    /// <summary>
    /// Creaza un nou Room
    /// </summary>
    public static class CreateRoom
    {
        const string endpoint = "/createRoom";
        public static async Task<int> SendRequest(int profToken)
        {
            //daca este 0 inseamna ca ceva rau sa 
            int result = 0;

            //folosit pt a sincroniza thread-urile
            SemaphoreSlim semaphore = new SemaphoreSlim(0);

            WebSocket ws = new WebSocket(Constants.url + endpoint + $"?profToken={profToken}");
            ws.OnMessage += (sender, args) =>
            {
                result = Convert.ToInt32(args.Data);
                semaphore.Release();
                ws.Close();
            };
            ws.OnError += (sender, args) =>
            {
                System.Diagnostics.Trace.WriteLine("createRoom websocket error with code: " + (args.Exception as WebSocketException).Code.ToString());
            };
            ws.OnOpen += (sender, e) => { ws.Send("0"); };
            //am putea folosi ConnectAsync dar nu este necesar
            ws.Connect();
            //asteptam ca conexiunea sa se inchida
            await semaphore.WaitAsync();
            return result;
        }
    }
}
