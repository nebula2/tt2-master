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

[assembly: ExportRenderer(typeof(Xamarin.Forms.ProgressBar), typeof(CustomProgressbarRenderer))]
namespace TT2Master.Droid
{
    public class CustomProgressbarRenderer : ProgressBarRenderer
    {
        public CustomProgressbarRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ProgressBar> e)
        {
            base.OnElementChanged(e);

            try
            {
                if(Control != null)
                {
                    Control.ScaleY = 6;
                }
            }
            catch (Exception)
            { }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) => base.OnElementPropertyChanged(sender, e);
    }
}