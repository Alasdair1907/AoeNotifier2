using AoeNotifier.Model.AoeNet;
using AoeNotifier.Model.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoeNotifier.Model
{
    class LobbyToDisplayLobbyConverter
    {
        public static List<DisplayLobby> FromLobbies(List<Lobby> lobbies)
        {
            List<DisplayLobby> res = new List<DisplayLobby>();
            List<Lobby> validLobbies = LobbyVerifier.GetValidLobbies(lobbies);

            for (int i = 0; i < validLobbies.Count; i++)
            {
                Lobby lobby = validLobbies[i];
                res.Add(FromLobby(lobby, (i % 2 == 0) ? Colors.LobbiesLightZebra : Colors.LobbiesDarkZebra));
            }
            

            return res;
        }
        public static DisplayLobby FromLobby(Lobby lobby, string color)
        {
            return new DisplayLobby()
            {
                Color = color,
                LobbyID = lobby.LobbyId,
                MatchID = lobby.MatchId,
                LobbyName = lobby.Name.Trim(),
                PlayerCount = lobby.NumPlayers,
                LobbySize = lobby.NumSlots,
                PlayersListStr = PlayerListToStr(lobby.Players)
            };
        }

        public static string PlayerListToStr(List<Player> players)
        {
            List<string> playerNames = (from player in players where player != null && !string.IsNullOrEmpty(player.Name) select player.Name).ToList();
            return string.Join(", ", playerNames);
        }
    }
}
