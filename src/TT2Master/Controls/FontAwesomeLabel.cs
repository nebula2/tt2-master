using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TT2Master
{
    public class FontAwesomeLabel : Label
    {
        public static readonly string FontAwesomeName = (string)Application.Current.Resources["FontAwesome"];

        //Parameterless constructor for XAML
        public FontAwesomeLabel()
        {
            FontFamily = FontAwesomeName;
        }

        public FontAwesomeLabel(string fontAwesomeLabel = null)
        {
            FontFamily = FontAwesomeName;
            Text = fontAwesomeLabel;
        }
    }

    public static class Icon
    {
        public static readonly string Reload = "&#xf0e2;";
        public static readonly string Add = "&#xf067;";
        public static readonly string Edit = "&#xf044;";
    }
}
