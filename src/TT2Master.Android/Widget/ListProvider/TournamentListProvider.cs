using Android.Content;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace TT2Master.Droid
{
    /// <summary>
    /// Provides List of tournament ranks
    /// </summary>
    public class TournamentListProvider : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
    {
        private List<TournamentMember> _listItemList = new List<TournamentMember>();
        private readonly Context _context;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="contextNew"></param>
        public TournamentListProvider(Context contextNew)
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
                _listItemList = TournamentInfo.Members;
            }
            catch (Exception ex)
            {
                WidgetLogger.WriteToLogFile($"Exception at NextTourneyListProvider.PopulateListItem(): {ex.Message}");
            }
        }

        public int Count => _listItemList != null ? _listItemList.Count : 0;

        public long GetItemId(int position) => position;

        /// <summary>
        /// Here the list items are created
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public RemoteViews GetViewAt(int position)
        {
            var remoteView = new RemoteViews(_context.PackageName, Resource.Layout.list_row);
            var listItem = _listItemList[position];

            //set Color
            var txtColor = listItem.IsMyself ? Android.Graphics.Color.Orange : Android.Graphics.Color.Black;

            remoteView.SetTextViewText(Resource.Id.rank, listItem.Rank);
            remoteView.SetTextColor(Resource.Id.rank, txtColor);

            remoteView.SetTextViewText(Resource.Id.player, listItem.Name);
            remoteView.SetTextColor(Resource.Id.player, txtColor);

            remoteView.SetTextViewText(Resource.Id.stage_num, listItem.Stage);
            remoteView.SetTextColor(Resource.Id.stage_num, txtColor);

            //remoteView.SetTextViewText(Resource.Id.maxLvl, listItem.MaxStage);

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