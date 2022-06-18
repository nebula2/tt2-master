using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TT2Master.Shared.Helper;

namespace TT2Master.Model.Clan
{
    public class TournamentResult : BindableBase
    {
        #region Properties
        private ObservableCollection<TournamentResultMember> _member = new ObservableCollection<TournamentResultMember>();
        public ObservableCollection<TournamentResultMember> Member { get => _member; set => SetProperty(ref _member, value); }

        #endregion

        #region private methods
        private void ParseClanMessage(string msg)
        {
            try
            {
                var jArr = JArray.Parse(msg);
                Member = new ObservableCollection<TournamentResultMember>();

                foreach (JObject item in jArr.Children())
                {
                    var tm = new TournamentResultMember
                    {
                        Id = item.GetValue("ID").ToString(),
                        Rank = JfTypeConverter.ForceInt(item.GetValue("rank").ToString()),
                        Name = item.GetValue("name").ToString(),
                        Stage = JfTypeConverter.ForceInt(item.GetValue("stage").ToString()),
                        Flag = item.GetValue("stage").ToString(),
                        UndisputedCount = JfTypeConverter.ForceInt(item.GetValue("undisputed_count").ToString()),
                        HighlightPlayer = JfTypeConverter.ForceBool(item.GetValue("highlight_player").ToString()),
                    };

                    Member.Add(tm);
                }
            }
            catch (Exception) { }
        }
        #endregion

        #region Ctor
        public TournamentResult(ClanMessage msg)
        {
            ParseClanMessage(msg.Message);
        }

        public TournamentResult() { }
        #endregion
    }
}
