using AoeNotifier.Model.AoeNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoeNotifier.Model.Util
{
    class LobbyVerifier
    {
        public static List<Lobby> GetValidLobbies(List<Lobby> lobbies)
        {
            List<Lobby> res = new List<Lobby>();

            if (lobbies == null)
            {
                return res;
            }


            foreach (Lobby lobby in lobbies)
            {
                if (LobbyIsValid(lobby))
                {
                    res.Add(lobby);
                }
            }

            return res;
        }

        public static bool LobbyIsValid(Lobby lobby)
        {
            if (lobby.Name == null)
            {
                return false;
            }

            if (lobby.LobbyId == null)
            {
                return false;
            }

            if (lobby.Players == null || lobby.Players.Count == 0)
            {
                return false;
            }

            return true;
        }
    }
}
