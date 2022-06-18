using Android.App;
using Android.Graphics;
using Android.Views;
using Java.IO;
using Plugin.CurrentActivity;
using System.IO;
using TT2Master.Droid;
using Xamarin.Forms;

/// <summary>
/// Implementation of Interface to access Filesystem
/// </summary>
[assembly: Dependency(typeof(ScreenshotHelper))]
namespace TT2Master.Droid
{
    public class ScreenshotHelper : ICreateScreenshot
    {
        private Activity _currentActivity;

        public byte[] CaptureContentResource()
        {
            _currentActivity = CrossCurrentActivity.Current.Activity as Activity;
            var rootView = _currentActivity.Window.DecorView.FindViewById(Android.Resource.Id.Content);//.RootView;
            var viewGroup = (ViewGroup)rootView;
            using (var screenshot = Bitmap.CreateBitmap(
                                    rootView.Width,
                                    rootView.Height,
                                    Bitmap.Config.Argb8888))
            {
                var canvas = new Canvas(screenshot);
                rootView.Draw(canvas);

                using (var stream = new MemoryStream())
                {
                    screenshot.Compress(Bitmap.CompressFormat.Jpeg, 90, stream);

                    SaveImage(stream.ToArray());

                    return stream.ToArray();
                }
            }
        }

        public byte[] CaptureContentResourceTest()
        {
            _currentActivity = CrossCurrentActivity.Current.Activity as Activity;
            var rootView = _currentActivity.Window.DecorView.FindViewById(Android.Resource.Id.Content);//.RootView;
            var viewGroup = (ViewGroup)rootView;
            var scroll = viewGroup.GetChildAt(0);

            using (var screenshot = Bitmap.CreateBitmap(
                                    scroll.Width,
                                    scroll.Height,
                                    Bitmap.Config.Argb8888))
            {
                var canvas = new Canvas(screenshot);
                scroll.Draw(canvas);

                using (var stream = new MemoryStream())
                {
                    screenshot.Compress(Bitmap.CompressFormat.Jpeg, 90, stream);

                    SaveImage(stream.ToArray());

                    return stream.ToArray();
                }
            }
        }

        public string Capture()
        {
            byte[] arr;
            //byte[] arr = CaptureDrawingCache();
            //arr = CaptureContentResource();
            arr = CaptureContentResourceTest();
            //TakeScreenShot();

            return SaveImage(arr);
        }

        /// <summary>
        /// Saves jpeg byteArray to filesystem
        /// </summary>
        /// <param name="imgArr"></param>
        private string SaveImage(byte[] imgArr)
        {
            string dir = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures).AbsolutePath;
            string path = System.IO.Path.Combine(dir, "pipikakaTest.jpg");

            //Check path and create if non existant
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            //Write
            using (var fileOutputStream = new FileOutputStream(path))
            {
                fileOutputStream.Write(imgArr);
            }

            return path;
        }
    }
}