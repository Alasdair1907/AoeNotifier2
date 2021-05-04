using AoeNotifier.Model;
using AoeNotifier.Model.AoeNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoeNotifier.Engine
{
    public class Main
    {
        public static LoadResult LoadLobbies(List<Filter> filtersList)
        {
            List<Lobby> lobbies;
            List<Lobby> matchingLobbies;
            List<DisplayLobby> displayLobbies;
            List<Player> players;

            LoadResult loadResult = new LoadResult();
            LobbyStats stats = new LobbyStats();

            try
            {
                lobbies = Fetcher.GetLobbies();
                matchingLobbies = Matching.GetMatchingLobbies(lobbies, filtersList);
                displayLobbies = LobbyToDisplayLobbyConverter.FromLobbies(matchingLobbies);
                players = lobbies.SelectMany(l => l.Players.Where(p => p != null && !string.IsNullOrWhiteSpace(p.Name))).ToList();
            }
            catch (Exception ex)
            {
                loadResult.ErrorMessage = "Error updating lobbies:\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                return loadResult;
            }

            stats.TotalLobbies = lobbies.Count;
            stats.MatchingLobbies = displayLobbies.Count;
            stats.TotalPlayers = players.Count;

            loadResult.LobbyStats = stats;
            loadResult.DisplayLobbies = displayLobbies;

            return loadResult;
        }

    }
}
