using AoeNotifier.Model;
using AoeNotifier.Model.AoeNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoeNotifier.Engine
{
    public class Matching
    {
        private static readonly string splitCharacters = "~`!@#$%^&*()_+{}[]\\|/;:\"\t \r\n,.<>-=?";
        private static readonly string whiteSpaceCharacters = "\t\r\n ";

        private static readonly char[] splitChars = splitCharacters.ToArray();
        private static readonly char[] whiteSpaceChars = whiteSpaceCharacters.ToArray();

        public static List<Lobby> GetMatchingLobbies(List<Lobby> lobbies, List<Filter> filters)
        {
            if (lobbies == null)
            {
                return new List<Lobby>();
            }

            if (filters == null || filters.Count == 0)
            {
                return new List<Lobby>();
            }

            List<Filter> positiveFilters = filters.Where(filter => filter.filterMode == FilterMode.Notify).ToList();
            List<Filter> negativeFilters = filters.Where(filter => filter.filterMode == FilterMode.Ignore).ToList();
            var positiveFiltersGroups = filters.GroupBy(f => f.GroupId);

            List<Lobby> matchedLobbies = new List<Lobby>();

            foreach (Lobby lobby in lobbies)
            {
                bool negativeMatch = false;

                foreach (Filter filter in negativeFilters)
                {
                    if (LobbyMatches(lobby, filter))
                    {
                        negativeMatch = true;
                        break;
                    }
                }

                if (negativeMatch)
                {
                    continue;
                }


                foreach (var filterGroup in positiveFiltersGroups)
                {
                    bool groupMatches = true;
                    foreach (Filter filter in filterGroup)
                    {
                        if (!LobbyMatches(lobby, filter))
                        {
                            groupMatches = false;
                            break;
                        }
                    }

                    if (!groupMatches)
                    {
                        continue;
                    }

                    matchedLobbies.Add(lobby);
                    break;
                }
            }

            return matchedLobbies;
        }


        public static bool LobbyMatches(Lobby lobby, Filter filter)
        {
            
            if (lobby == null || filter == null)
            {
                return false;
            }

            if (filter.filterType == FilterType.LobbyTitle)
            {
                return TextMatches(filter.Text, lobby.Name, filter.predicateType, filter.textMode);
            } 
            else if (filter.filterType == FilterType.PlayerName)
            {
                if (lobby.Players == null)
                {
                    return false;
                }

                foreach (Player player in lobby.Players)
                {
                    if (player == null)
                    {
                        continue;
                    }

                    if (TextMatches(filter.Text, player.Name, filter.predicateType, filter.textMode))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool TextMatches(string textFilter, string textAoe, PredicateType predicateType, TextMode textMode)
        {

            if (string.IsNullOrWhiteSpace(textFilter) || string.IsNullOrWhiteSpace(textAoe))
            {
                return false;
            }

            string caseTextFilter;
            string caseTextAoe;

            if (textMode == TextMode.CaseInsensitive)
            {
                caseTextFilter = textFilter.ToLower();
                caseTextAoe = textAoe.ToLower();
            } else
            {
                caseTextFilter = textFilter;
                caseTextAoe = textAoe;
            }

            switch (predicateType)
            {
                case PredicateType.FullMatch:
                    return string.Equals(caseTextFilter, caseTextAoe);
                case PredicateType.Contains:
                    return caseTextAoe.Contains(caseTextFilter);
                case PredicateType.AllWords:
                    return AllWords(caseTextAoe, caseTextFilter);
            }

            return false;
        }

        public static bool AllWords(string caseTextAoe, string caseTextFilter)
        {
            string[] wordsFilter = caseTextFilter.Split(whiteSpaceChars, StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in wordsFilter)
            {
                if (!WordSearch(caseTextAoe, word))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool WordSearch(string haystack, string needle)
        {
            if (string.IsNullOrWhiteSpace(haystack) || string.IsNullOrWhiteSpace(needle))
            {
                return false;
            }

            if (!haystack.Contains(needle))
            {
                return false;
            }

            string guid = Guid.NewGuid().ToString().Replace("-", "");
            string preparedHaystack = haystack.Replace(needle, guid);
            string[] words = preparedHaystack.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

            return words.Contains(guid);
        }
    }

}
