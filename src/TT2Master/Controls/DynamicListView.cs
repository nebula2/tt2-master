using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TT2Master.Model.Reporting;
using Xamarin.Forms;

namespace TT2Master.Controls
{
    public class DynamicListView : ListView
    {
        public static readonly BindableProperty ColumnConfigProperty = BindableProperty.Create(
            nameof(IsVisible), 
            typeof(ReportColumnConfig), 
            typeof(BindableToolbarItem)
            , null
            , BindingMode.OneWay
            , null
            , OnColumnConfigChanged
            );

        public ReportColumnConfig ColumnConfig { get; set; }

        public DynamicListView() : base(ListViewCachingStrategy.RetainElement)
        {

        }


        private static void OnColumnConfigChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if(newvalue != oldvalue)
            {
                var item = bindable as DynamicListView;
                Device.BeginInvokeOnMainThread(() => { item.RecreateUi(newvalue); });
            }
        }

        void RecreateUi(object newvalue)
        {
            if(newvalue == null)
            {
                return;
            }

            var colDef = newvalue as ReportColumnConfig;
            if(colDef == null)
            {
                Debug.WriteLine("invalid ReportColumnConfig passed");
                return;
            }

            if(colDef.Columns == null)
            {
                return;
            }

            var itemTemplate = new DataTemplate(() => 
            {
                var grid = new Grid();

                for (int i = 0; i < colDef.Columns.Count; i++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });

                    var content = new Label();
                    content.SetBinding(Label.TextProperty, colDef.Columns[i].DataField);

                    grid.Children.Add(content, i, 0);
                }

                return new ViewCell { View = grid };
            });

            ItemTemplate = itemTemplate;
        }
    }
}
