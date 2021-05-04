using AoeNotifier.Model.AoeNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoeNotifier.Model
{

    public class DisplayLobby
    {
        public string Color { get; set; }
        public string LobbyID { get; set; }
        public string MatchID { get; set; }
        public string LobbyName { get; set; }
        public int PlayerCount { get; set; }
        public int LobbySize { get; set; }
        public string PlayersListStr { get; set; }
    }
}
