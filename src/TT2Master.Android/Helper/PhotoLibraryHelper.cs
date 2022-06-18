using Android.Content;
using Android.Media;
using Android.OS;
using Android.Provider;
using Java.IO;
using System;
using System.Threading.Tasks;
using TT2Master.Droid;
using TT2Master.Droid.Loggers;
using TT2Master.Interfaces;

/// <summary>
/// Implementation of Interface to access Filesystem
/// </summary>
[assembly: Xamarin.Forms.Dependency(typeof(PhotoLibraryHelper))]
namespace TT2Master.Droid
{
    public class PhotoLibraryHelper : IPhotoLibrary
    {
        public async Task<(bool, string)> SavePhotoAsync(byte[] data, string folder, string filename)
        {
            string destinationFilename = "";
            try
            {
                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q)
                {
                    ContentResolver resolver = Android.App.Application.Context.ContentResolver;
                    ContentValues contentValues = new ContentValues();
                    contentValues.Put(MediaStore.MediaColumns.DisplayName, filename);
                    contentValues.Put(MediaStore.MediaColumns.MimeType, "image/png");
                    contentValues.Put(MediaStore.MediaColumns.RelativePath, "DCIM/" + folder);
                    var imageUri = resolver.Insert(MediaStore.Images.Media.ExternalContentUri, contentValues);

                    var descriptor = resolver.OpenFileDescriptor(imageUri, "w");
                    if (descriptor != null)
                    {
                        using var outputStream = new FileOutputStream(descriptor.FileDescriptor);
                        await outputStream.WriteAsync(data);
                    }

                    // Make sure it shows up in the Photos gallery promptly.
                    MediaScannerConnection.ScanFile(Android.App.Application.Context,
                                                    new string[] { imageUri.Path },
                                                    new string[] { "image/png", "image/jpeg" }, null);

                    destinationFilename = imageUri.ToString();
                }
                else
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    var picturesDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
#pragma warning restore CS0618 // Type or member is obsolete
                    var folderDirectory = picturesDirectory;

                    if (!string.IsNullOrEmpty(folder))
                    {
                        folderDirectory = new File(picturesDirectory, folder);
                        folderDirectory.Mkdirs();
                    }

                    using var bitmapFile = new File(folderDirectory, filename);
                    bitmapFile.CreateNewFile();

                    using (var outputStream = new FileOutputStream(bitmapFile))
                    {
                        await outputStream.WriteAsync(data);
                    }

                    // Make sure it shows up in the Photos gallery promptly.
                    MediaScannerConnection.ScanFile(Android.App.Application.Context,
                                                    new string[] { bitmapFile.Path },
                                                    new string[] { "image/png", "image/jpeg" }, null);

                    destinationFilename = bitmapFile.AbsolutePath;
                }
            }
            catch (Exception ex)
            {
                AutoServiceLogger.WriteToLogFile($"SavePhotoAsync ERROR on {Build.VERSION.SdkInt}: {ex.Message}\n{ex.Data}");
                return (false, ex.Message);
            }

            //destinationFilename = destinationFilename.EndsWith(filename)
            //    ? destinationFilename
            //    : destinationFilename.EndsWith('/')
            //        ? destinationFilename + filename
            //        : destinationFilename + $"/{filename}";

            return (true, destinationFilename);
        }
    }
}