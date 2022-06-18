using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;
using TT2Master.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(GetBitmapResourceHelper))]
namespace TT2Master.iOS
{
    public class GetBitmapResourceHelper : IGetBitmapResources
    {
        public SKBitmap GetDecodedResource(string resourceId)
        {

            var arr = GetAssetAsByteArray(resourceId);
            return SKBitmap.Decode(arr);


            //var path = Path.Combine(Environment.CurrentDirectory, $"{resourceId}.png");
            //Debug.WriteLine($"Exists (\"{path}\"): {File.Exists(path)}");

            //SKBitmap confettiBMP;
            //using (Stream fileStream = File.OpenRead(path))
            //using (var managedStream = new SKManagedStream(fileStream))
            //{
            //    confettiBMP = SKBitmap.Decode(managedStream);
            //}

            //return confettiBMP ?? new SKBitmap();
        }

        private byte[] GetAssetAsByteArray(string filename)
        {
            var image = UIImage.FromBundle(filename);
            using (var imageData = image.AsPNG())
            {
                var myByteArray = new byte[imageData.Length];
                System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, myByteArray, 0, Convert.ToInt32(imageData.Length));
                return myByteArray;
            }
        }
    }
}