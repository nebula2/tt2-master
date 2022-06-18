using Foundation;
using System;
using System.Threading.Tasks;
using TT2Master.Interfaces;
using TT2Master.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PhotoLibraryHelper))]
namespace TT2Master.iOS
{
    public class PhotoLibraryHelper : IPhotoLibrary
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<(bool, string)> SavePhotoAsync(byte[] data, string folder, string filename)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var img = new UIImage(NSData.FromArray(data));
            img.SaveToPhotosAlbum((image, error) =>
            {
                if (error != null)
                {
                    Console.WriteLine(error.ToString());
                }
            });

            return (true, "Gallery");
        }
    }
}