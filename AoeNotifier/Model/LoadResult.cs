using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoeNotifier.Model
{
    public class LoadResult
    {
        public string ErrorMessage { get; set; }

        public List<DisplayLobby> DisplayLobbies { get; set; }
        public LobbyStats LobbyStats { get; set; }
    }
}
