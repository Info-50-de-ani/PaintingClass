using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using System.Threading;
using System.Windows;
using System.Net.Http;

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

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(new Uri($"{Constants.urlHttp}{endpoint}?profToken={profToken}"));
            if (int.TryParse(await response.Content.ReadAsStringAsync(), out result))
                return result;
            else
                return 0;
        }
    }
}
