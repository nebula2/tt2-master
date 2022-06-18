using Android.Content;
using Xamarin.Forms;
using TT2Master.Droid;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(TT2MasterNavigationPageRenderer))]
namespace TT2Master.Droid
{
    public class TT2MasterNavigationPageRenderer : NavigationPageRenderer
    {
        private Android.Support.V7.Widget.Toolbar toolbar;

        public TT2MasterNavigationPageRenderer(Context context) : base(context)
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
        {
            base.OnElementChanged(e);
            //SetCustomView(e.NewElement.CurrentPage.GetType().Name);
        }

        private void SetCustomView(string view)
        {
            toolbar.Subtitle = " -> " + view;
            
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();


        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            //if (e.PropertyName.Equals("CurrentPage"))
            //{
            //    SetCustomView(((NavigationPage)sender).CurrentPage.GetType().Name);
            //}
        }

        public override void OnViewAdded(Android.Views.View child)
        {
            base.OnViewAdded(child);

            if (child.GetType() == typeof(Android.Support.V7.Widget.Toolbar))
            {
                toolbar = (Android.Support.V7.Widget.Toolbar)child;
            }
        }
    }
}
