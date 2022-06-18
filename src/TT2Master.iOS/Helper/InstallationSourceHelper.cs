using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using TT2Master.iOS;
using TT2Master.Interfaces;
using TT2Master.Model;

[assembly: Dependency(typeof(InstallationSourceHelper))]
namespace TT2Master.iOS
{
    public class InstallationSourceHelper : IGetInstallationSource
    {
        public InstallationSourceResult GetInstallationSource()
        {
            return new InstallationSourceResult("com.iOS.tmpmockPewPew") { Information = "temporary solution until store validation is handled" };
        }
    }
}