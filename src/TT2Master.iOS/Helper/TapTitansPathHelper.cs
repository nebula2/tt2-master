using System;
using TT2Master.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(TapTitansPathHelper))]
namespace TT2Master.iOS
{
    public class TapTitansPathHelper : ITapTitansPath
    {
        public string GetFileName() => "/Apps/Tap Titans 2/Documents/ISavableGlobal.adat";
        public string GetAbyssalFileName() => "/Apps/Tap Titans 2/Documents/ISavableGlobalChallenge.adat";

        public string ProcessPathString(string filepath)
        {
            throw new NotImplementedException();
        }
    }
}