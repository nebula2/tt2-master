using System;
using System.Collections.Generic;
using System.Text;
using TT2Master.Model.Dashboard;
using Xamarin.Forms;

namespace TT2Master.Views.Dashboard
{
    public class DashboardShortcutTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ContentTemplate { get; set; }
        public DataTemplate ContentlessTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (!(item.GetType()).IsSubclassOf(typeof(DashboardShortcut)))
            {
                throw new ArgumentException("item is not derived from DashboardShortcut!", nameof(item));
            }
            
            return GetDashboardTemplate(item);
        }

        private DataTemplate GetDashboardTemplate(dynamic item) => item.HasContent ? ContentTemplate : ContentlessTemplate;
    }
}
