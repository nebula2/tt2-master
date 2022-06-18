using System.Linq;
using Xamarin.Forms;

namespace TT2Master.Controls
{
    /// <summary>
    /// Toolbar with visibility
    /// see here: https://forums.xamarin.com/discussion/40622/found-a-way-to-make-toolbaritems-visible-invisible-without-a-custom-renderer
    /// </summary>
    public class BindableToolbarItem : ToolbarItem
    {
        public BindableToolbarItem() { }

        //NOTE: Default value is true, because toolbar items are by default visible when added to the Toolbar
        public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(BindableToolbarItem), true, BindingMode.OneWay, null, OnIsVisibleChanged);

        public bool IsVisible
        {
            get => (bool)GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        private static void OnIsVisibleChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var item = bindable as BindableToolbarItem;
            Device.BeginInvokeOnMainThread(() => { item.SetVisibility(oldvalue, newvalue); });
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            SetVisibility(false, IsVisible);
        }

        void SetVisibility(object oldvalue, object newvalue)
        {
            if (Parent == null) return;

            if(!(newvalue is bool isVisible))
            {
                return;
            }

            if (Parent is ContentPage cPage)
            {
                SetContentPageVisibility(cPage, isVisible);
            }

            if (Parent is TabbedPage tPage)
            {
                SetTabbedPageVisibility(tPage, isVisible);
            }
        }

        void SetContentPageVisibility(ContentPage page, bool newvalue)
        {
            var items = page.ToolbarItems;

            if (page.ToolbarItems == null)
            {
                return;
            }

            if (newvalue && !items.Contains(this))
            {
                // Find the insertion point (based on Priority of other toolbar items)
                var nextItem = items.FirstOrDefault(i => i.Priority > Priority);
                int idx = (nextItem != null) ? items.IndexOf(nextItem) : items.Count;
                // Insert this toolbar item
                items.Insert(idx, this);
            }
            else if (!newvalue && items.Contains(this))
            {
                items.Remove(this);
            }
        }

        void SetTabbedPageVisibility(TabbedPage page, bool newvalue)
        {
            var items = page.ToolbarItems;

            if (page.ToolbarItems == null)
            {
                return;
            }

            if (newvalue && !items.Contains(this))
            {
                // Find the insertion point (based on Priority of other toolbar items)
                var nextItem = items.FirstOrDefault(i => i.Priority > Priority);
                int idx = (nextItem != null) ? items.IndexOf(nextItem) : items.Count;
                // Insert this toolbar item
                items.Insert(idx, this);
            }
            else if (!newvalue && items.Contains(this))
            {
                items.Remove(this);
            }
        }
    }
}