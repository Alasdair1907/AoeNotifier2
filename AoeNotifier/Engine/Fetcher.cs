using AoeNotifier.Model.AoeNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AoeNotifier.Engine
{
    class Fetcher
    {
        public static readonly string lobbyRequest = "https://aoe2.net/api/lobbies?game=aoe2de";

        public static List<Lobby> GetLobbies()
        {
            string data;
            using (WebClient client = new MyWebClient())
            {
                client.Encoding = Encoding.UTF8;
                
                data = client.DownloadString(lobbyRequest);
            }


            if (string.IsNullOrWhiteSpace(data))
            {
                return new List<Lobby>();
            }

            List<Lobby> lobbies = JsonConvert.DeserializeObject<List<Lobby>>(data);
            return lobbies;
        }

        private class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest webRequest = base.GetWebRequest(address);
                webRequest.Timeout = 15 * 1000;
                return webRequest;
            }
        }


    }


}
