using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using System;
using System.IO;
using TT2Master.Droid.Loggers;

namespace TT2Master.Droid
{
    // TODO get filepath or do something to read this shit!!!

    public static class UriToPath
    {
        public static string GetActualPathFromFile(Android.Net.Uri uri)
        {
            bool isKitKat = Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat;
            string filePath = "";
            var context = Android.App.Application.Context;

            AutoServiceLogger.WriteToLogFile($"UriToPath.GetActualPathFromFile: isKitKat: {isKitKat}");

            if (isKitKat /*&& DocumentsContract.IsDocumentUri(Android.App.Application.Context, uri)*/)
            {
                AutoServiceLogger.WriteToLogFile($"UriToPath.GetActualPathFromFile: isDocumentUri");

                // ExternalStorageProvider
                if (IsExternalStorageDocument(uri))
                {
                    AutoServiceLogger.WriteToLogFile($"UriToPath.GetActualPathFromFile: isExternalStorageDocument");


                    string docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    string[] split = docId.Split(chars);
                    string type = split[0];

                    var splitUri = uri.Path.Split(chars);

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        return Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, splitUri[1]);
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                    else
                    {
                        if (Build.VERSION.SdkInt > BuildVersionCodes.Kitkat)
                        {
                            //getExternalMediaDirs() added in API 21
                            var extenal = context.GetExternalMediaDirs();
                            if (extenal.Length > 1)
                            {
                                filePath = extenal[1].AbsolutePath;
                                filePath = filePath.Substring(0, filePath.IndexOf("Android")) + splitUri[1];
                                //filePath = Path.Combine(filePath, splitUri[1].Substring(0, splitUri[1].IndexOf("Android")));
                            }
                        }
                        else
                        {
                            filePath = "/storage/" + type + "/" + split[1];
                        }
                        return Path.Combine(filePath);
                    }
                }
                // DownloadsProvider
                else if (IsDownloadsDocument(uri))
                {
                    AutoServiceLogger.WriteToLogFile($"UriToPath.GetActualPathFromFile: IsDownloadsDocument");


                    string id = DocumentsContract.GetDocumentId(uri);

                    Android.Net.Uri contentUri = ContentUris.WithAppendedId(
                                    Android.Net.Uri.Parse("content://downloads/public_downloads"), long.Parse(id));

                    //System.Diagnostics.Debug.WriteLine(contentUri.ToString());

                    return GetDataColumn(Android.App.Application.Context, contentUri, null, null);
                }
                // MediaProvider
                else if (IsMediaDocument(uri))
                {
                    AutoServiceLogger.WriteToLogFile($"UriToPath.GetActualPathFromFile: IsMediaDocument");


                    string docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    string[] split = docId.Split(chars);

                    string type = split[0];

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;
                    }

                    string selection = "_id=?";
                    string[] selectionArgs = new string[]
                    {
                        split[1]
                    };

                    return GetDataColumn(Android.App.Application.Context, contentUri, selection, selectionArgs);
                }
            }
            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                AutoServiceLogger.WriteToLogFile($"UriToPath.GetActualPathFromFile: IsMediaStore");

                // Return the remote address
                return IsGooglePhotosUri(uri) ? uri.LastPathSegment : GetDataColumn(Android.App.Application.Context, uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                AutoServiceLogger.WriteToLogFile($"UriToPath.GetActualPathFromFile: IsFile");

                return uri.Path;
            }

            return null;
        }

        [Android.Runtime.Register("ACTION_OPEN_DOCUMENT")]
        public static string GetDataColumn(Context context, Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            ICursor cursor = null;
            string column = "_data";
            string[] projection =
            {
                column
            };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(index);
                }
            }
            finally
            {
                if (cursor != null)
                {
                    cursor.Close();
                }
            }
            return null;
        }

        //Whether the Uri authority is ExternalStorageProvider.
        public static bool IsExternalStorageDocument(Android.Net.Uri uri) => "com.android.externalstorage.documents".Equals(uri.Authority);

        //Whether the Uri authority is DownloadsProvider.
        public static bool IsDownloadsDocument(Android.Net.Uri uri) => "com.android.providers.downloads.documents".Equals(uri.Authority);

        //Whether the Uri authority is MediaProvider.
        public static bool IsMediaDocument(Android.Net.Uri uri) => "com.android.providers.media.documents".Equals(uri.Authority);

        //Whether the Uri authority is Google Photos.
        public static bool IsGooglePhotosUri(Android.Net.Uri uri) => "com.google.android.apps.photos.content".Equals(uri.Authority);

    }
}