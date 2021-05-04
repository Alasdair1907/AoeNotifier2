using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoeNotifier.Model.AoeNet
{
    public class Player
    {

        [JsonProperty("profile_id")]
        public String ProfileId { get; set; }

        [JsonProperty("steam_id")]
        public String SteamId { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

    }
}
