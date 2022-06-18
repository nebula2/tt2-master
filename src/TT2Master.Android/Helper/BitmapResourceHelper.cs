using SkiaSharp;
using TT2Master.Droid;

/// <summary>
/// Implementation of Interface to access Filesystem
/// </summary>
[assembly: Xamarin.Forms.Dependency(typeof(BitmapResourceHelper))]
namespace TT2Master.Droid
{
    public class BitmapResourceHelper : IGetBitmapResources
    {
        public SKBitmap GetDecodedResource(string resourceId)
        {
            try
            {
                int resId = (int)typeof(Resource.Drawable).GetField(resourceId).GetValue(null);

                using var stream = Android.App.Application.Context.Resources.OpenRawResource(resId);
                return SKBitmap.Decode(stream);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}