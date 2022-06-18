using Xamarin.Forms;
using TT2Master.Droid;

[assembly: Dependency(typeof(DBPathHelper))]
namespace TT2Master.Droid
{
    public class DBPathHelper : IDBPath
    {
        public string DBPath(string dbname)
        {
            string path = System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.Personal);
            return System.IO.Path.Combine(path, dbname);
        }
    }
}