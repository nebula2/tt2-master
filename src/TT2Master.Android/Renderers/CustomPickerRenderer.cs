using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TT2Master;
using TT2Master.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomPickerRenderer))]
namespace TT2Master.Droid
{
    public class CustomPickerRenderer : PickerRenderer
    {
        public CustomPickerRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            try
            {
                if (Control != null)
                {
                    SetColors();
                }
            }
            catch (Exception)
            { }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            try
            {
                if (e.PropertyName == nameof(Entry.IsEnabled))
                {
                    SetColors();
                }
            }
            catch (Exception)
            { }
        }

        private void SetColors() => Control?.SetTextColor(Element.IsEnabled ? Element.TextColor.ToAndroid() : new Android.Graphics.Color(82, 90, 187));//Control.SetBackgroundColor(Element.IsEnabled ? Element.BackgroundColor.ToAndroid() : Android.Graphics.Color.Pink);
    }
}