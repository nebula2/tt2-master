using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TT2Master.Droid
{
    /// <summary>
    /// Provides List of tournament ranks
    /// </summary>
    public class NextTourneyListProvider : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
    {
        private List<NextTournament> _listItemList = new List<NextTournament>();
        private Context _context;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="contextNew"></param>
        public NextTourneyListProvider(Context contextNew)
        {
            _context = contextNew;
            PopulateListItem();
        }

        /// <summary>
        /// Creates Items
        /// </summary>
        private void PopulateListItem()
        {
            try
            {
                #region Tournament
                if (TournamentInfo.Model == null)
                {
                    TournamentInfo.HardResetModel();
                }

                _listItemList = new List<NextTournament>();

                var tournament = new NextTournament()
                {
                    header = $"Next Tournament is {TournamentInfo.Model.PrizeText}",
                    content = $"Starting: {TypeConverter.ForceDate(TournamentInfo.Model.StartTime).Date.ToString("yyyy.MM.dd")} with {TournamentInfo.Model.BonusAmount}x {TournamentInfo.Model.BonusType}"
                };

                _listItemList.Add(tournament);

                WidgetLogger.WriteToLogFile($"Added {tournament.header}\n{tournament.content}");

                #endregion

                #region equip

                if(TournamentInfo.ExtraEquipmentDrops == null || TournamentInfo.MyStage == null)
                {
                    TournamentInfo.HardResetEquip();
                }

                var equip = new NextTournament()
                {
                    header = "To get all equipment drops this run",
                    content = TournamentInfo.ExtraEquipmentDrops.Count == 0 ? $"Got all. You are at {TournamentInfo.MyStage.CurrentStage}" : $"Reach Stage: {TournamentInfo.ExtraEquipmentDrops.Max()}. You are at {TournamentInfo.MyStage.CurrentStage}"
                };

                _listItemList.Add(equip);

                WidgetLogger.WriteToLogFile($"Added {equip.header}\n{equip.content}");
                #endregion

                #region Daily
                //var daily = new NextTournament();

                if (TournamentInfo.MyAchievments == null)
                {
                    TournamentInfo.HardResetArchievement();
                }

                if (TournamentInfo.MyAchievments.Where(x => !x.DailyAchievementCollected).Count() == 0)
                {
                    _listItemList.Add(new NextTournament()
                    {
                        header = "Daily achievement",
                        content = "Collected",
                    });
                }
                else
                {
                    foreach (var item in TournamentInfo.MyAchievments)
                    {
                        // jump over if already collected
                        if (item.DailyAchievementCollected)
                        {
                            continue;
                        }

                        var daily = new NextTournament()
                        {
                            header = $"Daily {item.CurrentDailyAchievement}",
                            content = $"{item.CurrentDailyAchievementProgress} / {item.CurrentDailyAchievementTotal}",
                        };

                        _listItemList.Add(daily);
                        WidgetLogger.WriteToLogFile($"Added {daily.header}\n{daily.content}");
                    }

                    //daily.header = "Daily achievement";
                    //daily.content = $"{TournamentInfo.MyAchievement.CurrentDailyAchievement}: {TournamentInfo.MyAchievement.CurrentDailyAchievementProgress} / {TournamentInfo.MyAchievement.CurrentDailyAchievementTotal}";
                }

                //_listItemList.Add(daily);

                
                #endregion
            }
            catch (Exception ex)
            {
                WidgetLogger.WriteToLogFile($"Exception at NextTourneyListProvider.PopulateListItem(): {ex.Message}");
            }
        }

        public int Count => _listItemList.Count;

        public long GetItemId(int position) => position;

        /// <summary>
        /// Here the list items are created
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public RemoteViews GetViewAt(int position)
        {
            var remoteView = new RemoteViews(_context.PackageName, Resource.Layout.info_row);
            var listItem = _listItemList[position];

            remoteView.SetTextViewText(Resource.Id.header, listItem.header);
            remoteView.SetTextViewText(Resource.Id.content, listItem.content);

            return remoteView;
        }

        public RemoteViews LoadingView => null;

        public int ViewTypeCount => 1;

        public bool HasStableIds => true;

        public void OnCreate() { }

        public void OnDataSetChanged() { }

        public void OnDestroy() { }
    }
}