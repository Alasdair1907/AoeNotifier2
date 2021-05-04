using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AoeNotifier.Util;

namespace AoeNotifier.Model
{
    public enum FilterType
    {
        [Description("LOBBY TITLE")]
        LobbyTitle,
        [Description("PLAYER NAME")]
        PlayerName
    }

    public enum PredicateType
    {
        [Description("FULL MATCH")]
        FullMatch,
        [Description("CONTAINS")]
        Contains,
        [Description("ALL WORDS")]
        AllWords
    }

    public enum TextMode
    {
        [Description("CASE SENSITIVE")]
        CaseSensitive,
        [Description("CASE INSENSITIVE")]
        CaseInsensitive
    }

    public enum FilterMode
    {
        [Description("NOTIFY")]
        Notify,
        [Description("IGNORE")]
        Ignore
    }

    public class Filter
    {
        public FilterType filterType;
        public PredicateType predicateType;
        public TextMode textMode;
        public FilterMode filterMode;

        public int id;
        public int GroupId { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public string FilterType { get => Tools.GetEnumDescription(this.filterType); }
        public string PredicateType { get => Tools.GetEnumDescription(this.predicateType); }
        public string TextMode { get => Tools.GetEnumDescription(this.textMode); }
        public string FilterMode { get => Tools.GetEnumDescription(this.filterMode); }
    }

}
