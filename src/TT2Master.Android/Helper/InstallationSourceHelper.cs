using TT2Master.Droid;
using TT2Master.Interfaces;
using TT2Master.Model;
using Xamarin.Forms;

/// <summary>
/// Implementation of Interface to access Filesystem
/// </summary>
[assembly: Dependency(typeof(InstallationSourceHelper))]
namespace TT2Master.Droid
{
    public class InstallationSourceHelper : IGetInstallationSource
    {
        public InstallationSourceResult GetInstallationSource()
        {
            // TODO change this with API Version 30 

            try
            {
                var installer = Android.App.Application.Context.PackageManager.GetInstallerPackageName(Android.App.Application.Context.PackageName);

                var result = new InstallationSourceResult(installer);

                if (string.IsNullOrWhiteSpace(installer))
                {
                    result.Information = "no vendor specified. Seems like a side loaded installation";
                    return result; // side loaded
                }

                result.Information = "vendor was specified.";
                return result;
            }
            catch (System.Exception ex)
            {

                return new InstallationSourceResult(null) { Information = $"ERROR at GetInstallationSource: {ex.Message}" };
            }
        }
    }
}
