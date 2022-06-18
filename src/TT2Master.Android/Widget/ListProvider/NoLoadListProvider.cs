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
    public class NoLoadListProvider : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
    {
        private List<TourneyWidgetNoLoad> _listItemList = new List<TourneyWidgetNoLoad>();
        private Context _context;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="contextNew"></param>
        public NoLoadListProvider(Context contextNew)
        {
            _context = contextNew;
            PopulateListItem();
        }

        /// <summary>
        /// Creates Items
        /// </summary>
        private void PopulateListItem()
        {
            _listItemList.Add(new TourneyWidgetNoLoad()
            {
                header = "Could not get Data",
                content = TT2Widget.ShitHappenedHere
            });
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