using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using TT2Master.iOS;

[assembly: Dependency(typeof(DBPathHelper))]
namespace TT2Master.iOS
{
    public class DBPathHelper : IDBPath
    {
        public string DBPath(string dbname)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = System.IO.Path.Combine(docFolder, "..", "Library");

            if (!System.IO.Directory.Exists(libFolder))
            {
                System.IO.Directory.CreateDirectory(libFolder);
            }

            return System.IO.Path.Combine(libFolder, dbname);
        }
    }
}